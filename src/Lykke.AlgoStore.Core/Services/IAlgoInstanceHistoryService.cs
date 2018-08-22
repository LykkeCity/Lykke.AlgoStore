using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.Service.AlgoTrades.Client.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoInstanceHistoryService
    {
        Task<IEnumerable<Candle>> GetCandlesAsync(string assetPair, CandlePriceType priceType, CandleTimeInterval timeInterval, DateTime fromMoment, DateTime toMoment, ModelStateDictionary errorsDictionary, CancellationToken ctoken);
        Task<AlgoInstanceTradeResponse> GetTradesAsync(string instanceId, string tradedAssetId, DateTime fromMoment, DateTime toMoment);
        Task<IEnumerable<FunctionChartingUpdate>> GetFunctionsAsync(string instanceId, DateTime fromMoment, DateTime toMoment, string clientId, ModelStateDictionary errorsDictionary);
        Task<IEnumerable<QuoteChartingUpdate>> GetQuotesAsync(string instanceId, string asetPair, DateTime fromMoment, DateTime toMoment, bool? IsBuy, string clientId, ModelStateDictionary errorsDictionary);
        Task<string> GetAssetPairName(string assetPairId);
    }
}
