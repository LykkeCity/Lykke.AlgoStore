using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IWebSocketHandler webSocketHandler)
        {
            var processed = await ProcessWebSocketRequest(context, webSocketHandler);
            if (!processed)
            {
                await this._next(context);
            }
        }

        protected virtual async Task<bool> ProcessWebSocketRequest(
            HttpContext context, 
            IWebSocketHandler webSocketHandler)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var connected = await webSocketHandler.OnConnected(context);
                if (connected)
                {
                    await webSocketHandler.Listen();
                }
                return true;
            }
            return false;
        }
    }
}
