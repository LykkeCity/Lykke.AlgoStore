using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Lykke.Common.Log;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class CandlesWebSocketHandler : WebSocketHandlerBase<CandleChartingUpdate>
    {
        public CandlesWebSocketHandler(RealTimeDataSourceBase<CandleChartingUpdate> candlesListener, ILogFactory log, WebSocketAuthenticationManager authManager) : base(log, candlesListener, authManager)
        {
            ConfigureDataSource = () =>
            {
                DataListener.Configure(ConnectionId);
            };
        }
    }
}
