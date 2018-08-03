using Common.Log;
using MessagePack;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.Api.RealTimeStreaming.Filters;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Lykke.Common.Log;
using Lykke.AlgoStore.Api.RealTimeStreaming.Stomp;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.Service.Session;

#pragma warning disable 618

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public interface IWebSocketHandler
    {
        Task<bool> OnConnected(HttpContext context);
        Task StreamData();
        Task Listen();
        Task OnDisconnected(Exception exception = null);
    }


    public class WebSocketHandlerBase<T> : IWebSocketHandler
    {
        protected WebSocket Socket;
        protected IObservable<T> Messages;
        protected readonly ILog Log;
        protected Action ConfigureDataSource;
        protected string ConnectionId;
        protected readonly RealTimeDataSourceBase<T> DataListener;
        
        private readonly IAlgoClientInstanceRepository _clientInstanceRepository;
        private readonly IClientSessionsClient _clientSessionsClient;

        private string _clientId;

        public WebSocketHandlerBase(
            ILogFactory logFactory,
            RealTimeDataSourceBase<T> dataListener,
            IClientSessionsClient clientSessionsClient,
            IAlgoClientInstanceRepository clientInstanceRepository)
        {
            Log = logFactory.CreateLog(Constants.LogComponent);
            DataListener = dataListener;
            Messages = DataListener.Select(t => t);
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

            if (ConfigureDataSource != null)
            {
                ConfigureDataSource.Invoke();
            }
            else
            {
                DataListener.Configure(ConnectionId, new DataFilter(ConnectionId));
            }

            var requestType = context.Request.PathBase.Value;
            Log.Info(nameof(WebSocketHandlerBase<T>), $"Web socket {requestType} connection opened. InstanceId = {ConnectionId}.", nameof(OnConnected));
            return true;
        }

        public virtual async Task StreamData()
        {
            await Task.Run(async () =>
            {
                IObserver<T> observer = Observer.Create<T>(
                    onNext: async message =>
                    {
                        var msgJson = MessagePackSerializer.ToJson(message, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
                        var bytes = Encoding.UTF8.GetBytes(msgJson);

                        if (Socket.State == WebSocketState.Open)
                        {
                            try
                            {
                                //await Socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            catch (WebSocketException ex)
                            {
                                Log.Error(ex, "Error while attempting to send message over socket for ConnectionId={ConnectionId}. Message={msgJson}", nameof(StreamData));
                                await OnDisconnected(ex);
                            }
                        }
                    },
                    onError: async ex =>
                    {
                        Log.Error(ex, $"Error while reading data from source for ConnectionId={ConnectionId}.", nameof(StreamData));
                        await OnDisconnected(ex);
                    },
                    onCompleted: async () =>
                    {
                        Log.Info(nameof(WebSocketHandlerBase<T>), $"Data source for ConnectionId={ConnectionId} completed.", "DataCompleted");
                        DataListener.TokenSource.Cancel();
                    });

                using (Messages.Subscribe(observer))
                {
                    try
                    {
                        while (Socket.State == WebSocketState.Open)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    }
                    catch (Exception ex)
                    {
                        await OnDisconnected(ex);
                    }
                    finally
                    {
                        DataListener.TokenSource.Cancel();
                    }
                }
            });
        }

        public virtual async Task Listen()
        {
            try
            {
                var session = new StompSession(Socket);
                session.AddAuthenticationCallback(AuthenticateAsync);

                await session.Listen();

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
                DataListener.TokenSource.Cancel();
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
                DataListener.TokenSource.Cancel();

                if (Socket.CloseStatus == WebSocketCloseStatus.EndpointUnavailable)
                {
                    Socket.Dispose();
                    Log.Info(nameof(WebSocketHandlerBase<T>), $"WebSocket ConnectionId={ConnectionId} closed due to client disconnect. ", nameof(OnDisconnected));
                    return;
                }

                if (Socket.State == WebSocketState.Open || Socket.State == WebSocketState.CloseReceived || Socket.State == WebSocketState.CloseSent)
                {
                    if (exception == null)
                    {
                        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket closure requested.", CancellationToken.None);
                        Log.Info(nameof(WebSocketHandlerBase<T>), $"WebSocket ConnectionId={ConnectionId} closed.", nameof(OnDisconnected));
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

            return true;
        }

        public async Task<bool> AuthenticateAsync(string clientId, string token)
        {
            var session = await _clientSessionsClient.GetAsync(token);
            if (session != null && session.ClientId == clientId)
            {
                Log.Info(nameof(WebSocketHandlerBase<T>),
                    $"Successful websocket authentication for clientId {clientId}." +
                    $" {GetType().Name}", "AuthenticateOK");

                _clientId = clientId;

                return true;
            }

            return false;
        }
    }
}
