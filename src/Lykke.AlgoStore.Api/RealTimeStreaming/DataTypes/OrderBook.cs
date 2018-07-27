using System;
using Lykke.AlgoStore.Algo.Charting;
using MessagePack;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes
{
    [MessagePackObject(keyAsPropertyName:true)]
    public class OrderBook : IChartingUpdate
    {
        [Key(0)]
        public string Source { get; set; }

        [Key(1)]
        public string Asset { get; set; }

        [Key(2)]
        public DateTime Timestamp { get; set; }

        [Key(3)]
        public PriceVolume[] Asks { get; set; }

        [Key(4)]
        public PriceVolume[] Bids { get; set; }

        public string InstanceId { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class PriceVolume
    {
        [Key(0)]
        public decimal Price { get; set; }

        [Key(1)]
        public decimal Volume { get; set; }
    }
}
