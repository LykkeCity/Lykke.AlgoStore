using Common.Log;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
        protected WebSocket Socket;
        protected IObservable<CandleChartingUpdate> Messages;
        protected readonly ILog Log;
        protected Action ConfigureDataSource;
        protected string ConnectionId;

        private readonly RealTimeDataSource<CandleChartingUpdate> _candleSource;
        private readonly RealTimeDataSource<FunctionChartingUpdate> _functionSource;
        private readonly RealTimeDataSource<TradeChartingUpdate> _tradeSource;
        
        private readonly IAlgoClientInstanceRepository _clientInstanceRepository;
        private readonly IClientSessionsClient _clientSessionsClient;

        private readonly Dictionary<(string, string), string> _subscribedQueues
            = new Dictionary<(string, string), string>();

        private string _clientId;
        private StompSession _stompSession;

        private bool _candlesSubscribed;
        private bool _functionsSubscribed;
        private bool _tradesSubscribed;

        public WebSocketHandler(
            ILogFactory logFactory,
            RealTimeDataSource<CandleChartingUpdate> candleRealTimeSource,
            RealTimeDataSource<FunctionChartingUpdate> functionRealTimeSource,
            RealTimeDataSource<TradeChartingUpdate> tradeRealTimeSource,
            IClientSessionsClient clientSessionsClient,
            IAlgoClientInstanceRepository clientInstanceRepository)
        {
            Log = logFactory.CreateLog(Constants.LogComponent);

            _candleSource = candleRealTimeSource;
            _functionSource = functionRealTimeSource;
            _tradeSource = tradeRealTimeSource;

            _clientSessionsClient = clientSessionsClient;
            _clientInstanceRepository = clientInstanceRepository;
        }

        public virtual async Task<bool> OnConnected(HttpContext context)
        {
            ConnectionId = context.Request.Query[Constants.InstanceIdIdentifier];

            if (String.IsNullOrWhiteSpace(ConnectionId))
            {
                throw new HttpRequestException("Incorrect instance id supplied.");
            }

            if (context.WebSockets.WebSocketRequestedProtocols.Contains("v12.stomp"))
                Socket = await context.WebSockets.AcceptWebSocketAsync("v12.stomp");
            else if (context.WebSockets.WebSocketRequestedProtocols.Contains("v11.stomp"))
                Socket = await context.WebSockets.AcceptWebSocketAsync("v11.stomp");
            else
                Socket = await context.WebSockets.AcceptWebSocketAsync();

            SubscribeAll();

            var requestType = context.Request.PathBase.Value;
            Log.Info(nameof(WebSocketHandler), $"Web socket {requestType} connection opened. InstanceId = {ConnectionId}.", nameof(OnConnected));
            return true;
        }

        public virtual async Task Listen()
        {
            try
            {
                _stompSession = new StompSession(Socket);
                _stompSession.AddAuthenticationCallback(AuthenticateAsync);
                _stompSession.AddSubscriptionCallback(SubscribeAsync);

                await _stompSession.Listen();

                while (Socket.State == WebSocketState.Open)
                {
                    var result = await ReceiveFullMessage(CancellationToken.None);

                    if (result.ReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await OnDisconnected();
                    }
                }
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

        protected async Task<(WebSocketReceiveResult ReceiveResult, IEnumerable<byte> Message)> ReceiveFullMessage(CancellationToken cancelToken)
        {
            WebSocketReceiveResult response;
            var message = new List<byte>();

            var buffer = new byte[Constants.WebSocketRecieveBufferSize];
            do
            {
                response = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancelToken);
                message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
            } while (!response.EndOfMessage);

            return (ReceiveResult: response, Message: message);
        }

        public virtual async Task OnDisconnected(Exception exception = null)
        {
            try
            {
                UnsubscribeAll();

                if (Socket.CloseStatus == WebSocketCloseStatus.EndpointUnavailable)
                {
                    Socket.Dispose();
                    Log.Info(nameof(WebSocketHandler), $"WebSocket ConnectionId={ConnectionId} closed due to client disconnect. ", nameof(OnDisconnected));
                    return;
                }

                if (Socket.State == WebSocketState.Open || Socket.State == WebSocketState.CloseReceived || Socket.State == WebSocketState.CloseSent)
                {
                    if (exception == null)
                    {
                        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket closure requested.", CancellationToken.None);
                        Log.Info(nameof(WebSocketHandler), $"WebSocket ConnectionId={ConnectionId} closed.", nameof(OnDisconnected));
                    }
                    else
                    {
                        await Socket.CloseAsync(WebSocketCloseStatus.InternalServerError, Constants.WebSocketErrorMessage, CancellationToken.None);
                        Log.Warning($"WebSocket ConnectionId={ConnectionId} closed due to error.", exception, nameof(OnDisconnected));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while trying to close WebSocket ConnectionId={ConnectionId}. Current socket state {Socket?.State}, closure status {Socket?.CloseStatus}  ", nameof(OnDisconnected));
            }
        }

        private async Task<bool> SubscribeAsync(string queueName)
        {
            var splits = queueName.Split('/');

            if (splits.Length < 3) return false;

            if (splits[0] != "instance") return false;

            var instanceId = splits[1];

            if (!await _clientInstanceRepository.ExistsAlgoInstanceDataWithClientIdAsync(_clientId, instanceId))
                return false;

            switch(splits[2])
            {
                case "candles":
                    if (_candlesSubscribed) return false;
                    _candlesSubscribed = true;
                    break;
                case "functions":
                    if (_functionsSubscribed) return false;
                    _functionsSubscribed = true;
                    break;
                case "trades":
                    if (_tradesSubscribed) return false;
                    _tradesSubscribed = true;
                    break;
                default:
                    return false;
            }

            _subscribedQueues.Add((instanceId, splits[2]), queueName);

            return true;
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
            _candleSource.Subscribe(OnCandleReceived);
            _functionSource.Subscribe(OnFunctionReceived);
            _tradeSource.Subscribe(OnTradeReceived);
        }

        private void UnsubscribeAll()
        {
            _candleSource.Unsubscribe(OnCandleReceived);
            _functionSource.Unsubscribe(OnFunctionReceived);
            _tradeSource.Unsubscribe(OnTradeReceived);
        }

        private async Task OnCandleReceived(CandleChartingUpdate candleUpdate)
        {
            if (!_candlesSubscribed) return;

            if (!_subscribedQueues.TryGetValue((candleUpdate.InstanceId, "candles"), out string queue)) return;

            await _stompSession.SendToQueueAsync(queue, candleUpdate);
        }

        private async Task OnFunctionReceived(FunctionChartingUpdate functionUpdate)
        {
            if (!_functionsSubscribed) return;

            if (!_subscribedQueues.TryGetValue((functionUpdate.InstanceId, "functions"), out string queue)) return;

            await _stompSession.SendToQueueAsync(queue, functionUpdate);
        }

        private async Task OnTradeReceived(TradeChartingUpdate tradeUpdate)
        {
            if (!_tradesSubscribed) return;

            if (!_subscribedQueues.TryGetValue((tradeUpdate.InstanceId, "trades"), out string queue)) return;

            await _stompSession.SendToQueueAsync(queue, tradeUpdate);
        }
    }
}
