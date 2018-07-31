using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoInstanceHistoryService
    {
        Task<IEnumerable<Candle>> GetCandlesAsync(string assetPair, CandlePriceType priceType, CandleTimeInterval timeInterval, DateTime fromMoment, DateTime toMoment, ModelStateDictionary errorsDictionary, CancellationToken ctoken);
        Task<IEnumerable<AlgoInstanceTrade>> GetTradesAsync(string instanceId, string tradedAssetId, DateTime fromMoment, DateTime toMoment, ModelStateDictionary errorsDictionary);
    }
}
