using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using System.Reactive.Linq;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class CandlesWebSocketHandler : WebSocketHandlerBase<CandleChartingUpdate>
    {
        private readonly RealTimeDataSourceBase<CandleChartingUpdate> _candlesListener;

        public CandlesWebSocketHandler(RealTimeDataSourceBase<CandleChartingUpdate> candlesListener, ILog log) : base(log)
        {
            _candlesListener = candlesListener;
            Messages = candlesListener.Select(t => t);
            SendCancelToDataSource = () =>
            {
                _candlesListener.TokenSource.Cancel();
            };
            ConfigureDataSource = () =>
            {
                _candlesListener.Configure(ConnectionId);
            };
        }
    }
}
