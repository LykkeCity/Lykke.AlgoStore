using System.Threading.Tasks;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware
{
    public class DummyWebSocketsMiddleware :  WebSocketMiddlewareBase<OrderBook>
    {
        private readonly RequestDelegate _next;

        public DummyWebSocketsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DummyWebSocketHandler dummyWebSocketHandler)
        {
            var processed = await ProcessWebSocketRequest(context, dummyWebSocketHandler);

            if (!processed)
            {
                await this._next(context);
            }


        }
    }
}
