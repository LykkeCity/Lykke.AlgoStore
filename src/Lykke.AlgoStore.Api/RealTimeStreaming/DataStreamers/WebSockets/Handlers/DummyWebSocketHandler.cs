using Common.Log;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes;
using Lykke.AlgoStore.Api.RealTimeStreaming.Filters;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Microsoft.AspNetCore.Http;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
#pragma warning disable 618

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class DummyWebSocketHandler : WebSocketHandlerBase<OrderBook>
    {
        public DummyWebSocketHandler(RealTimeDataSourceBase<OrderBook> orderBooksListener, ILog log) : base(log, orderBooksListener)
        {
           
        }

        public override async Task<bool> OnConnected(HttpContext context)
        {
            Socket = await context.WebSockets.AcceptWebSocketAsync();
            var assetId = context.Request.Query["AssetId"];
            ConnectionId = context.Request.Query[Constants.InstanceIdIdentifier];
            var infoMsg = $"Connection opened. ConnectionId = {ConnectionId}. AssetId= {assetId}";

            DataListener.Configure(ConnectionId, !string.IsNullOrWhiteSpace(assetId) ? new DataFilter(String.Empty, assetId) : null);
            await Log.WriteInfoAsync(nameof(DummyWebSocketHandler), nameof(OnConnected), infoMsg);
            return true;
        }
    }
}
