using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware
{
    public class WebSocketMiddleware<T> : WebSocketMiddlewareBase<T>
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, WebSocketHandlerBase<T> webSocketHandler)
        {
            var processed = await ProcessWebSocketRequest(context, webSocketHandler);
            if (!processed)
            {
                await this._next(context);
            }
        }

        

    }
}
