﻿using System;
using System.Net;
using System.Threading.Tasks;
using AzureStorage.Tables;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware
{
    public class WebSocketMiddleware<T> where T : IWebSocketHandler
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, T webSocketHandler)
        {
            var processed = await ProcessWebSocketRequest(context, webSocketHandler);
            if (!processed)
            {
                await this._next(context);
            }
        }

        protected virtual async Task<bool> ProcessWebSocketRequest(HttpContext context, T webSocketHandler)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    var connected = await webSocketHandler.OnConnected(context);
                    if (connected)
                    {
                        await webSocketHandler.Listen();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Unable to process connection");
                    throw;
                }
            }
            return false;
        }
    }
}
