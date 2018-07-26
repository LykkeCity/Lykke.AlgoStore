using System;
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
                    if (String.IsNullOrWhiteSpace(context.Request.Query[Constants.InstanceIdIdentifier]))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync("Incorrect InstanceId");
                        return true;
                    }

                    var connected = await webSocketHandler.OnConnected(context);
                    if (connected)
                    {
                        var outbandData = webSocketHandler.StreamData();
                        var inboundData = webSocketHandler.ListenForClosure();

                        await Task.WhenAny(outbandData, inboundData);
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
