using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.Filters;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers
{
    public class TradesWebSocketHandler : WebSocketHandlerBase<TradeChartingUpdate>
    {
        private readonly RealTimeDataSourceBase<TradeChartingUpdate> _tradesListener;

        public TradesWebSocketHandler(RealTimeDataSourceBase<TradeChartingUpdate> tradesListener, ILog log) : base(log)
        {
            _tradesListener = tradesListener;
            Messages = tradesListener.Select(t => t);
            SendCancelToDataSource = () =>
            {
                _tradesListener.TokenSource.Cancel();
            };
            ConfigureDataSource = () =>
            {
                _tradesListener.Configure(ConnectionId, new DataFilter(ConnectionId));
            };
        }
    }
}
