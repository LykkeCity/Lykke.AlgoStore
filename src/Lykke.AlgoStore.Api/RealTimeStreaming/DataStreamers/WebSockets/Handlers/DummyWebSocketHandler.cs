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
        private readonly RealTimeDataSourceBase<OrderBook> _orderBooksListener;

        public DummyWebSocketHandler(RealTimeDataSourceBase<OrderBook> orderBooksListener, ILog log) : base(log)
        {
            _orderBooksListener = orderBooksListener;
            Messages = orderBooksListener.Select(t => t);
            SendCancelToDataSource = () =>
            {
                _orderBooksListener.TokenSource.Cancel();
            };
        }

        public override async Task<bool> OnConnected(HttpContext context)
        {
            Socket = await context.WebSockets.AcceptWebSocketAsync();
            var assetId = context.Request.Query["AssetId"];
            ConnectionId = context.Request.Query["InstanceId"];
            var infoMsg = $"Connection opened. ConnectionId = {ConnectionId}.";

            if (!string.IsNullOrWhiteSpace(assetId))
            {
                _orderBooksListener.SupplyDataFilter(new DataFilter(String.Empty, assetId));
                infoMsg = String.Concat(infoMsg, " AssetId=", assetId);
            }
            await Log.WriteInfoAsync(nameof(DummyWebSocketHandler), nameof(OnConnected), infoMsg);
            return true;
        }
    }
}
