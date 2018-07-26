﻿using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware
{
    public class CandlesWebSocketsMiddleware : WebSocketMiddlewareBase<CandleChartingUpdate>
    {
        private readonly RequestDelegate _next;

        public CandlesWebSocketsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, CandlesWebSocketHandler candleWebSocketHandler)
        {
            var processed = await ProcessWebSocketRequest(context, candleWebSocketHandler);
            if (!processed)
            {
                await this._next(context);
            }
        }
    }
}
