﻿using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes
{
    [MessagePackObject]
    public class OrderBook : BaseDataModel
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
    }

    [MessagePackObject]
    public class PriceVolume
    {
        [Key(0)]
        public decimal Price { get; set; }

        [Key(1)]
        public decimal Volume { get; set; }
    }
}
