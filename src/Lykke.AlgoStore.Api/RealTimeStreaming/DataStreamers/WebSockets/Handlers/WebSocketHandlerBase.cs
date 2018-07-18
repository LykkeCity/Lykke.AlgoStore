using Common.Log;
using Lykke.Common.Log;
using MessagePack;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable 618

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class WebSocketHandlerBase<T>
    {
        protected WebSocket Socket;
        protected IObservable<T> Messages;
        protected readonly ILog Log;
        protected Action SendCancelToDataSource;
        protected string ConnectionId;

        public WebSocketHandlerBase(ILog log)
        {
            Log = log;
        }

        public virtual async Task<bool> OnConnected(HttpContext context)
        {
            Socket = await context.WebSockets.AcceptWebSocketAsync();
            return true;
        }

        public virtual async Task StreamData()
        {
            await Task.Run(async () =>
            {
                IObserver<T> observer = Observer.Create<T>(
                    onNext: async message =>
                    {
                        var msgJson = MessagePackSerializer.ToJson(message);
                        var bytes = Encoding.UTF8.GetBytes(msgJson);

                        if (Socket.State == WebSocketState.Open)
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
                        else
                        {
                            SendCancelToDataSource();
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
                        SendCancelToDataSource();
                    });

                using (Messages.Subscribe(observer))
                {
                    while (Socket.State == WebSocketState.Open)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    SendCancelToDataSource();
                }
            });
        }

        public virtual async Task ListenForClosure()
        {
            while (Socket.State == WebSocketState.Open)
            {
                var buffer = new byte[Constants.WebSocketRecieveBufferSize];
                var result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await Log.WriteInfoAsync(nameof(WebSocketHandlerBase<T>), nameof(ListenForClosure), $"WebSocket close request received from client for ConnectionId={ConnectionId}");
                    await OnDisconnected();
                }
            }
        }

        protected virtual async Task OnDisconnected(Exception exception = null)
        {
            SendCancelToDataSource();

            if (Socket.State == WebSocketState.Open || Socket.State == WebSocketState.CloseReceived || Socket.State == WebSocketState.CloseSent)
            {
                if (exception == null)
                {
                    await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket closure requested.", CancellationToken.None);
                    await Log.WriteInfoAsync(nameof(WebSocketHandlerBase<T>), nameof(OnDisconnected), $"WebSocket ConnectionId={ConnectionId} closed.");
                }
                else
                {
                    await Socket.CloseAsync(WebSocketCloseStatus.InternalServerError, $"{exception}", CancellationToken.None);
                    await Log.WriteWarningAsync(nameof(WebSocketHandlerBase<T>), nameof(OnDisconnected), $"WebSocket ConnectionId={ConnectionId} closed due to error.", exception);
                }
            }
        }
    }
}
