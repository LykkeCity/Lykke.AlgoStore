using Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes;
using Lykke.AlgoStore.Api.RealTimeStreaming.Filters;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.Common.Log;
using Lykke.Service.Session;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

#pragma warning disable 618

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class DummyWebSocketHandler : WebSocketHandlerBase<OrderBook>
    {
        public DummyWebSocketHandler(RealTimeDataSourceBase<OrderBook> candlesListener, ILogFactory log,
            IClientSessionsClient sessionClient, IAlgoClientInstanceRepository instanceRepo) : base(log, candlesListener, sessionClient, instanceRepo)
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
