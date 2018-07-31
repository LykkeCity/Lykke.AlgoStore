using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using System.Reactive.Linq;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class CandlesWebSocketHandler : WebSocketHandlerBase<CandleChartingUpdate>
    {
        public CandlesWebSocketHandler(RealTimeDataSourceBase<CandleChartingUpdate> candlesListener, ILog log, WebSocketAuthenticationManager authManager) : base(log, candlesListener, authManager)
        {
            ConfigureDataSource = () =>
            {
                DataListener.Configure(ConnectionId);
            };
        }
    }
}
