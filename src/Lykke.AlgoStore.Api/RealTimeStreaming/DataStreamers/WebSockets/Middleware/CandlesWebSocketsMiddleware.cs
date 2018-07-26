using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware
{
    public class CandlesWebSocketsMiddleware : WebSocketMiddlewareBase
    {
        private readonly RequestDelegate _next;

        public CandlesWebSocketsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, CandlesWebSocketHandler candleWebSocketHandler)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                if (String.IsNullOrWhiteSpace(context.Request.Query[Constants.InstanceIdIdentifier]))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync("Incorrect InstanceId");
                    return;
                }

                await ProcessWebSocketRequest(context, candleWebSocketHandler);
            }
            else
            {
                await this._next(context);
            }
        }
    }
}
