using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.Filters;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using System.Reactive.Linq;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class FunctionsWebSocketHandler : WebSocketHandlerBase<FunctionChartingUpdate>
    {
        private readonly RealTimeDataSourceBase<FunctionChartingUpdate> _functionValuesListener;

        public FunctionsWebSocketHandler(RealTimeDataSourceBase<FunctionChartingUpdate> functionValuesListener, ILog log) : base(log)
        {
            _functionValuesListener = functionValuesListener;
            Messages = functionValuesListener.Select(t => t);
            SendCancelToDataSource = () =>
            {
                _functionValuesListener.TokenSource.Cancel();
            };
            ConfigureDataSource = () =>
            {
                _functionValuesListener.Configure(ConnectionId, new DataFilter(ConnectionId));
            };
        }
    }
}
