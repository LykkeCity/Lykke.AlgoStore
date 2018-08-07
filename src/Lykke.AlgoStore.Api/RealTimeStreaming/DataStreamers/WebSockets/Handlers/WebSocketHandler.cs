using Common.Log;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Lykke.Common.Log;
using Lykke.AlgoStore.Api.RealTimeStreaming.Stomp;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.Service.Session;
using Lykke.AlgoStore.Algo.Charting;
using System.Linq;

#pragma warning disable 618

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public interface IWebSocketHandler
    {
        Task<bool> OnConnected(HttpContext context);
        Task Listen();
        Task OnDisconnected(Exception exception = null);
    }

    public class WebSocketHandler : IWebSocketHandler
    {
        private static readonly Dictionary<(string, string), uint> _clientSubscribedInstances
            = new Dictionary<(string, string), uint>();

        protected WebSocket Socket;
        protected IObservable<CandleChartingUpdate> Messages;
        protected readonly ILog Log;
        protected Action ConfigureDataSource;

        private readonly RealTimeDataSource<CandleChartingUpdate> _candleSource;
        private readonly RealTimeDataSource<FunctionChartingUpdate> _functionSource;
        private readonly RealTimeDataSource<TradeChartingUpdate> _tradeSource;
        
        private readonly IAlgoClientInstanceRepository _clientInstanceRepository;
        private readonly IClientSessionsClient _clientSessionsClient;

        private readonly uint _maxConnectionsPerClient;
        private readonly uint _maxInstancesPerClient;

        private readonly object _sync = new object();

        private readonly Dictionary<(string, string), string> _subscribedQueues
            = new Dictionary<(string, string), string>();

        private string _clientId;
        private StompSession _stompSession;

        private bool _dataSourceSubscribed;

        public WebSocketHandler(
            ILogFactory logFactory,
            RealTimeDataSource<CandleChartingUpdate> candleRealTimeSource,
            RealTimeDataSource<FunctionChartingUpdate> functionRealTimeSource,
            RealTimeDataSource<TradeChartingUpdate> tradeRealTimeSource,
            IClientSessionsClient clientSessionsClient,
            IAlgoClientInstanceRepository clientInstanceRepository,
            uint maxInstancesPerClient,
            uint maxConnectionsPerClient)
        {
            Log = logFactory.CreateLog(Constants.LogComponent);

            _candleSource = candleRealTimeSource;
            _functionSource = functionRealTimeSource;
            _tradeSource = tradeRealTimeSource;

            _clientSessionsClient = clientSessionsClient;
            _clientInstanceRepository = clientInstanceRepository;

            _maxInstancesPerClient = maxInstancesPerClient;
            _maxConnectionsPerClient = maxConnectionsPerClient;
        }

        public virtual async Task<bool> OnConnected(HttpContext context)
        {
            if (context.WebSockets.WebSocketRequestedProtocols.Contains("v12.stomp"))
                Socket = await context.WebSockets.AcceptWebSocketAsync("v12.stomp");
            else if (context.WebSockets.WebSocketRequestedProtocols.Contains("v11.stomp"))
                Socket = await context.WebSockets.AcceptWebSocketAsync("v11.stomp");
            else
                Socket = await context.WebSockets.AcceptWebSocketAsync();

            var requestType = context.Request.PathBase.Value;

            _stompSession = new StompSession(Socket, Log, _maxConnectionsPerClient);
            _stompSession.AddAuthenticationCallback(AuthenticateAsync);
            _stompSession.AddSubscribeCallback(SubscribeAsync);
            _stompSession.AddUnsubscribeCallback(UnsubscribeAsync);
            _stompSession.AddDisconnectCallback(async () => await OnDisconnected());

            return true;
        }

        public virtual async Task Listen()
        {
            try
            {
                await _stompSession.Listen();
            }
            catch (Exception ex)
            {
                await OnDisconnected(ex);
            }
            finally
            {
                UnsubscribeAll();
            }
        }

        public virtual async Task OnDisconnected(Exception exception = null)
        {
            try
            {
                UnsubscribeAll();

                if (Socket.CloseStatus == WebSocketCloseStatus.EndpointUnavailable)
                {
                    Socket.Dispose();
                    Log.Info(nameof(WebSocketHandler), $"WebSocket ConnectionId={_clientId} closed due to client disconnect. ", nameof(OnDisconnected));
                    return;
                }

                if (Socket.State == WebSocketState.Open || Socket.State == WebSocketState.CloseReceived || Socket.State == WebSocketState.CloseSent)
                {
                    if (exception == null)
                    {
                        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket closure requested.", CancellationToken.None);
                        Log.Info(nameof(WebSocketHandler), $"WebSocket ConnectionId={_clientId} closed.", nameof(OnDisconnected));
                    }
                    else
                    {
                        await Socket.CloseAsync(WebSocketCloseStatus.InternalServerError, Constants.WebSocketErrorMessage, CancellationToken.None);
                        Log.Warning($"WebSocket ConnectionId={_clientId} closed due to error.", exception, nameof(OnDisconnected));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while trying to close WebSocket ConnectionId={_clientId}. Current socket state {Socket?.State}, closure status {Socket?.CloseStatus}  ", nameof(OnDisconnected));
            }
        }

        private async Task<bool> SubscribeAsync(string queueName)
        {
            var splits = await ValidateQueueName(queueName);

            if (splits == null) return false;

            if (_subscribedQueues.ContainsKey((splits[1], splits[2]))) return false;

            if(!_subscribedQueues.Any(kvp => kvp.Key.Item1 == splits[1]))
            {
                if (!IncrementInstanceUsage(_clientId, splits[1]))
                    return false;
            }

            _subscribedQueues.Add((splits[1], splits[2]), queueName);

            SubscribeAll();

            return true;
        }

        private async Task UnsubscribeAsync(string queueName)
        {
            var splits = await ValidateQueueName(queueName);

            if (splits == null) return;

            var key = (splits[1], splits[2]);

            if (!_subscribedQueues.ContainsKey(key)) return;

            _subscribedQueues.Remove((splits[1], splits[2]));

            if (!_subscribedQueues.Any(kvp => kvp.Key.Item1 == splits[1]))
                DecrementInstanceUsage(_clientId, splits[1]);

            if (!_subscribedQueues.Any()) UnsubscribeAll();
        }

        private async Task<string[]> ValidateQueueName(string queueName)
        {
            var splits = queueName.Split('/');

            if (splits.Length < 3) return null;

            if (splits[0] != "instance") return null;

            var instanceId = splits[1];

            if (!await _clientInstanceRepository.ExistsAlgoInstanceDataWithClientIdAsync(_clientId, instanceId))
                return null;

            return splits;
        }

        private async Task<bool> AuthenticateAsync(string clientId, string token)
        {
            var session = await _clientSessionsClient.GetAsync(token);
            if (session != null && session.ClientId == clientId)
            {
                Log.Info(nameof(WebSocketHandler),
                    $"Successful websocket authentication for clientId {clientId}." +
                    $" {GetType().Name}", "AuthenticateOK");

                _clientId = clientId;

                return true;
            }

            return false;
        }

        private void SubscribeAll()
        {
            if (_dataSourceSubscribed) return;

            _candleSource.Subscribe(OnCandleReceived);
            _functionSource.Subscribe(OnFunctionReceived);
            _tradeSource.Subscribe(OnTradeReceived);

            _dataSourceSubscribed = true;
        }

        private void UnsubscribeAll()
        {
            if (!_dataSourceSubscribed) return;

            _candleSource.Unsubscribe(OnCandleReceived);
            _functionSource.Unsubscribe(OnFunctionReceived);
            _tradeSource.Unsubscribe(OnTradeReceived);

            _dataSourceSubscribed = false;
        }

        private async Task OnCandleReceived(CandleChartingUpdate candleUpdate)
        {
            if (!_subscribedQueues.TryGetValue((candleUpdate.InstanceId, "candles"), out string queue)) return;

            await _stompSession.SendToQueueAsync(queue, candleUpdate);
        }

        private async Task OnFunctionReceived(FunctionChartingUpdate functionUpdate)
        {
            if (!_subscribedQueues.TryGetValue((functionUpdate.InstanceId, "functions"), out string queue)) return;

            await _stompSession.SendToQueueAsync(queue, functionUpdate);
        }

        private async Task OnTradeReceived(TradeChartingUpdate tradeUpdate)
        {
            if (!_subscribedQueues.TryGetValue((tradeUpdate.InstanceId, "trades"), out string queue)) return;

            await _stompSession.SendToQueueAsync(queue, tradeUpdate);
        }

        private bool IncrementInstanceUsage(string clientId, string instanceId)
        {
            lock(_sync)
            {
                var key = (clientId, instanceId);

                if (!_clientSubscribedInstances.ContainsKey(key))
                {
                    if (_clientSubscribedInstances.Count >= _maxInstancesPerClient)
                        return false;

                    _clientSubscribedInstances.Add(key, 0);
                }

                _clientSubscribedInstances[key] += 1;

                return true;
            }
        }

        private void DecrementInstanceUsage(string clientId, string instanceId)
        {
            lock(_sync)
            {
                var key = (clientId, instanceId);

                if (!_clientSubscribedInstances.ContainsKey(key)) return;

                if (_clientSubscribedInstances[key] > 0)
                    _clientSubscribedInstances[key] -= 1;

                if (_clientSubscribedInstances[key] == 0)
                    _clientSubscribedInstances.Remove(key);
            }
        }
    }
}
