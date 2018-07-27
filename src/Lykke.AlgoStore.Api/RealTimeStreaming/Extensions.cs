using System;
using System.Collections.Generic;
using System.Text;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes;
using Lykke.AlgoStore.Api.RealTimeStreaming.Filters;

namespace Lykke.AlgoStore.Api.RealTimeStreaming
{
    public static class Extensions
    {
        public static bool IsAllowedByFilter(this IChartingUpdate data, DataFilter filter)
        {
            if (filter == null) return true;

            if (!String.IsNullOrWhiteSpace(filter.InstanceId) && filter.InstanceId != data.InstanceId) return false;

            

            if (data is OrderBook book) //TODO temporaty code - remove this if when Dummy (OrderBook) data streaming no longer needed
            {
                if (filter.AssetId != null && filter.AssetId != book.Asset) return false;
            }
            //--------------------------------------

            return true;
        }
    }
}
