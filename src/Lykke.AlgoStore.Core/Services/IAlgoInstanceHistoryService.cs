using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoInstanceHistoryService
    {
        Task<IEnumerable<Candle>> GetCandlesAsync(string assetPair, CandlePriceType priceType, CandleTimeInterval timeInterval, DateTime fromMoment, DateTime toMoment, ModelStateDictionary errorsDictionary, CancellationToken ctoken);
    }
}
