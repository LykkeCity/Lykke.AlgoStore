using System.Threading.Tasks;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware
{
    public class WebSocketMiddlewareBase
    {
        protected virtual async Task<bool> ProcessWebSocketRequest<T>(HttpContext context, WebSocketHandlerBase<T> webSocketHandler)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var connected = await webSocketHandler.OnConnected(context);
                if (connected)
                {
                    var outbandData = webSocketHandler.StreamData();
                    var inboundData = webSocketHandler.ListenForClosure();

                    await Task.WhenAny(outbandData, inboundData);
                    return true;
                }
            }
            return false;
        }
    }
}
