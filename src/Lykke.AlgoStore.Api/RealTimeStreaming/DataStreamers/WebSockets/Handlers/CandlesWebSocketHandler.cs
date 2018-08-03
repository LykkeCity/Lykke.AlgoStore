using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.Common.Log;
using Lykke.Service.Session;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class CandlesWebSocketHandler : WebSocketHandlerBase<CandleChartingUpdate>
    {
        public CandlesWebSocketHandler(RealTimeDataSourceBase<CandleChartingUpdate> candlesListener, ILogFactory log,
            IClientSessionsClient sessionClient, IAlgoClientInstanceRepository instanceRepo) : base(log, candlesListener, sessionClient, instanceRepo)
        {
            ConfigureDataSource = () =>
            {
                DataListener.Configure(ConnectionId);
            };
        }
    }
}
