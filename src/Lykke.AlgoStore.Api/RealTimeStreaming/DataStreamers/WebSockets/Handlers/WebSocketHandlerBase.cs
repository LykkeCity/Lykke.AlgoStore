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
        protected readonly WebSocketAuthenticationManager _authManager;

        public WebSocketHandlerBase(ILog log, RealTimeDataSourceBase<T> dataListener, WebSocketAuthenticationManager authManager)
        {
            Log = log;
            DataListener = dataListener;
            Messages = DataListener.Select(t => t);
            _authManager = authManager;
            _authManager.StartSession(this);
        }

        public virtual async Task<bool> OnConnected(HttpContext context)
        {
            ConnectionId = context.Request.Query[Constants.InstanceIdIdentifier];

            if (String.IsNullOrWhiteSpace(ConnectionId))
            {
                throw new HttpRequestException("Incorrect instance id supplied.");
            }

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
            await Log.WriteInfoAsync(nameof(DummyWebSocketHandler), nameof(OnConnected), $"Web socket connection for {requestType} opened. ConnectionId = {ConnectionId}.");
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

                        if (Socket.State == WebSocketState.Open && _authManager.IsAuthenticated())
                        {
                            try
                            {
                                await Socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            catch (WebSocketException ex)
                            {
                                await Log.WriteErrorAsync(nameof(WebSocketHandlerBase<T>), "Error while attempting to send message over socket for ConnectionId={ConnectionId}. Message={msgJson}", ex);
                                await OnDisconnected(ex);
                            }
                        }
                    },
                    onError: async ex =>
                    {
                        await Log.WriteErrorAsync(nameof(WebSocketHandlerBase<T>), "Error while reading data from source for ConnectionId={ConnectionId}.", ex);
                        await OnDisconnected(ex);
                    },
                    onCompleted: async () =>
                    {
                        await Log.WriteInfoAsync(nameof(WebSocketHandlerBase<T>), nameof(StreamData), $"Data source for ConnectionId={ConnectionId} completed.");
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
                    catch (WebSocketException ex)
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
                while (Socket.State == WebSocketState.Open)
                {
                    var result = await ReceiveFullMessage(CancellationToken.None);

                    if (result.ReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await OnDisconnected();
                    }
                    else if (!_authManager.IsAuthenticated())
                    {
                        var message = Encoding.UTF8.GetString(result.Message.ToArray());
                        await _authManager.AuthenticateAsync(message);
                    }
                }
            }
            catch (WebSocketException ex)
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
                    await Log.WriteInfoAsync(nameof(WebSocketHandlerBase<T>), nameof(OnDisconnected), $"WebSocket ConnectionId={ConnectionId} closed due to client disconnect. " + exception);
                    return;
                }

                if (Socket.State == WebSocketState.Open || Socket.State == WebSocketState.CloseReceived || Socket.State == WebSocketState.CloseSent)
                {
                    if (exception == null)
                    {
                        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket closure requested.", CancellationToken.None);
                        await Log.WriteInfoAsync(nameof(WebSocketHandlerBase<T>), nameof(OnDisconnected), $"WebSocket ConnectionId={ConnectionId} closed.");
                    }
                    else
                    {
                        if ((exception as WebSocketException)?.Message == _authManager.UNAUTHORIZED_MESSAGE)
                        {
                            await Socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, _authManager.UNAUTHORIZED_MESSAGE ?? Constants.WebSocketErrorMessage, CancellationToken.None);
                        }
                        else
                        {
                            await Socket.CloseAsync(WebSocketCloseStatus.InternalServerError, Constants.WebSocketErrorMessage, CancellationToken.None);
                        }
                        
                        await Log.WriteWarningAsync(nameof(WebSocketHandlerBase<T>), nameof(OnDisconnected), $"WebSocket ConnectionId={ConnectionId} closed due to error.", exception);
                    }
                }
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(WebSocketHandlerBase<T>), nameof(OnDisconnected), $"Error while trying to close WebSocket ConnectionId={ConnectionId}. Current socket state {Socket?.State}, closure status {Socket?.CloseStatus}  ", ex);
            }
        }
    }
}
