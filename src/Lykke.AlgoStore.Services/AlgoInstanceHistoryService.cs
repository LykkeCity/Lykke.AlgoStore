using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Services;
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

        public AlgoInstanceHistoryService(ICandleshistoryservice candlesHistoryService, ILog log) : base (log, nameof(AlgoInstanceHistoryService))
        {
            this._candlesHistoryService = candlesHistoryService;
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

            var candles = response.History.Select(c => c.ToAlgoCandle());

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
