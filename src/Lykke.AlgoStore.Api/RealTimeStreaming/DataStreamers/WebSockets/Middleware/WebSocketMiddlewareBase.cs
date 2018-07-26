using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware
{
    public class WebSocketMiddlewareBase<T>
    {
        protected virtual async Task<bool> ProcessWebSocketRequest(HttpContext context, WebSocketHandlerBase<T> webSocketHandler)
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
                    //return true; //its was a web socket request, dont pass the request to the next middleware
                    throw;
                }
            }
            return false;
        }
    }
}
