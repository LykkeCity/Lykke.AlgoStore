using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes;
using Lykke.Common.Log;
using MessagePack;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class WebSocketHandlerBase<T>
    {
        protected WebSocket Socket;
        protected IObservable<T> Messages;
        protected readonly ILog Log;
        protected Action SendCancelToDataSource;

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
                        await OnDisconnected(ex);
                    },
                    onCompleted: () =>
                    {
                        SendCancelToDataSource();
                    });

                using (Messages.Subscribe(observer))
                {
                    while (Socket.State == WebSocketState.Open)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
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

                }
                else
                {
                    await Socket.CloseAsync(WebSocketCloseStatus.InternalServerError, $"{exception}", CancellationToken.None);
                    Log.Error(exception, Constants.WebSocketErrorMessage);
                }
            }
        }
    }
}
