using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes;
using Lykke.AlgoStore.Api.RealTimeStreaming.Filters;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Lykke.Common.Log;
using MessagePack;
using Microsoft.AspNetCore.Http;

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
            if (!string.IsNullOrWhiteSpace(context.Request.Query["AssetId"]))
            {
                _orderBooksListener.SupplyDataFilter(new DataFilter(String.Empty, context.Request.Query["AssetId"]));
            }
            return true;
        }
    }
}
