using System;
using System.Collections.Generic;
using System.Text;
using CandleTimeIntervalLykkeService = Lykke.Service.CandlesHistory.Client.Models.CandleTimeInterval;
using CandlePriceTypeLykkeService = Lykke.Service.CandlesHistory.Client.Models.CandlePriceType;

namespace Lykke.AlgoStore.Services.Utils
{
    public static class CandleDataTypeMapper
    {
        private static readonly Dictionary<CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval, CandleTimeIntervalLykkeService> TimeIntervalMap = 
        new Dictionary<CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval, CandleTimeIntervalLykkeService>
        {
            [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Sec] = CandleTimeIntervalLykkeService.Sec,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Minute] = CandleTimeIntervalLykkeService.Minute,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Min5] = CandleTimeIntervalLykkeService.Min5,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Min15] = CandleTimeIntervalLykkeService.Min15,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Min30] = CandleTimeIntervalLykkeService.Min30,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Hour] = CandleTimeIntervalLykkeService.Hour,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Hour4] = CandleTimeIntervalLykkeService.Hour4,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Hour6] = CandleTimeIntervalLykkeService.Hour6,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Hour12] = CandleTimeIntervalLykkeService.Hour12,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Day] = CandleTimeIntervalLykkeService.Day,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Week] = CandleTimeIntervalLykkeService.Week,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Month] = CandleTimeIntervalLykkeService.Month,
        };

        private static readonly Dictionary<CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType, CandlePriceTypeLykkeService> PriceTypeMap =
            new Dictionary<CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType, CandlePriceTypeLykkeService>
            {
                [CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType.Ask] = CandlePriceTypeLykkeService.Ask,
                [CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType.Bid] = CandlePriceTypeLykkeService.Bid,
                [CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType.Mid] = CandlePriceTypeLykkeService.Mid,
                [CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType.Trades] = CandlePriceTypeLykkeService.Trades
            };


        public static Dictionary<CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval, Lykke.Service.CandlesHistory.Client.Models.CandleTimeInterval> TimeInterval() => TimeIntervalMap;
        public static Dictionary<CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType, Lykke.Service.CandlesHistory.Client.Models.CandlePriceType> PriceType() => PriceTypeMap;
    }
}
