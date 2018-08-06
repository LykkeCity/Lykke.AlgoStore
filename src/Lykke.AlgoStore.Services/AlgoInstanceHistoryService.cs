using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.AlgoTrades.Client;
using Lykke.AlgoStore.Service.History.Client;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.CandlesHistory.Client.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.Operations;
using Candle = Lykke.AlgoStore.Algo.Candle;
using CandlePriceType = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType;
using CandleTimeInterval = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval;

namespace Lykke.AlgoStore.Services
{
    public class AlgoInstanceHistoryService : BaseAlgoStoreService, IAlgoInstanceHistoryService
    {
        private readonly ICandleshistoryservice _candlesHistoryService;
        private readonly IAlgoTradesClient _tradesHistoryService;
        private readonly IHistoryClient _functionHistoryService;

        public AlgoInstanceHistoryService(ICandleshistoryservice candlesHistoryService,
                                          IAlgoTradesClient tradesHistoryService,
                                          IHistoryClient functionHistoryService,
                                          ILog log) : base (log, nameof(AlgoInstanceHistoryService))
        {
            this._candlesHistoryService = candlesHistoryService;
            this._tradesHistoryService = tradesHistoryService;
            this._functionHistoryService = functionHistoryService;
        }

        public async Task<IEnumerable<AlgoInstanceTrade>> GetTradesAsync(string instanceId, string tradedAssetId, DateTime fromMoment, DateTime toMoment, ModelStateDictionary errorsDictionary)
        {
            var trades = await _tradesHistoryService.GetAlgoInstanceTradesByPeriod(instanceId, tradedAssetId, fromMoment, toMoment);

            if (trades == null || trades.Error != null || trades.Records == null)
            {
                errorsDictionary.AddModelError("ServiceError", trades?.Error?.Message ?? "Unknown");
                return null;
            }

            var result = trades.Records.Select(AutoMapper.Mapper.Map<AlgoInstanceTrade>);

            return result;
        }

        public async Task<IEnumerable<FunctionChartingUpdate>> GetFunctionsAsync(string instanceId, DateTime fromMoment, DateTime toMoment, ModelStateDictionary errorsDictionary)
        {
            var functions = await _functionHistoryService.GetFunctionValues(instanceId, fromMoment, toMoment);

            if (functions == null)
            {
                errorsDictionary.AddModelError("ServiceError", "Unknown");
                return null;
            }

            var result = functions.Select(AutoMapper.Mapper.Map<Lykke.AlgoStore.Algo.Charting.FunctionChartingUpdate>);

            return result;
        }

        public async Task<IEnumerable<Candle>> GetCandlesAsync(string assetPair, CandlePriceType priceType, CandleTimeInterval timeInterval, DateTime fromMoment, DateTime toMoment, ModelStateDictionary errorsDictionary, CancellationToken ctoken)
        {
            if (!IsCandlesHistoryRequestValid(assetPair, priceType, timeInterval, fromMoment, toMoment, errorsDictionary))
            {
                return null;
            }

            var httpResponse= await _candlesHistoryService.GetCandlesHistoryOrErrorWithHttpMessagesAsync(assetPair, CandleDataTypeMapper.PriceType()[priceType], CandleDataTypeMapper.TimeInterval()[timeInterval], fromMoment, toMoment, null, ctoken);

            var response = httpResponse.ParseHttpResponse<CandlesHistoryResponseModel>(errorsDictionary);

            if (response == null || !errorsDictionary.IsValid)
            {
                return null;
            }

            var candles = response.History.Select(AutoMapper.Mapper.Map<Candle>);

            return candles;
        }

        public bool IsCandlesHistoryRequestValid(string assetPair, CandlePriceType priceType, CandleTimeInterval timeInterval, DateTime fromMoment, DateTime toMoment, ModelStateDictionary errorsDictionary)
        {
            if (!CandleDataTypeMapper.PriceType().TryGetValue(priceType, out var priceTypeService))
            {
                errorsDictionary.AddModelError("priceType", $"Unsupported priceType: {priceType}");
                return false;
            }
            if (!CandleDataTypeMapper.TimeInterval().TryGetValue(timeInterval, out var timeIntervalService))
            {
                errorsDictionary.AddModelError("timeInterval", $"Unsupported timeInterval: {timeInterval}");
                return false;
            }
            if (String.IsNullOrWhiteSpace(assetPair))
            {
                errorsDictionary.AddModelError("assetPair", $"AssetPair can't be empty.");
                return false;
            }
            if (fromMoment < new DateTime(1990, 1, 1))
            {
                errorsDictionary.AddModelError("fromMoment", $"fromMoment is too far in the past.");
                return false;
            }
            if (toMoment < fromMoment)
            {
                errorsDictionary.AddModelError("toMoment", $"toMoment cant be earlier than fromMoment.");
                return false;
            }
            return true;
        }
    }
}
