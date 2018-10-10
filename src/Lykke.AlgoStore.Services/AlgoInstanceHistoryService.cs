using Common.Log;
using JetBrains.Annotations;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Service.AlgoTrades.Client;
using Lykke.AlgoStore.Service.AlgoTrades.Client.Models;
using Lykke.AlgoStore.Service.History.Client;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Service.Assets.Client.ReadModels;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.CandlesHistory.Client.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candle = Lykke.AlgoStore.Algo.Candle;
using CandlePriceType = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandlePriceType;
using CandleTimeInterval = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval;

namespace Lykke.AlgoStore.Services
{
    public class AlgoInstanceHistoryService : BaseAlgoStoreService, IAlgoInstanceHistoryService
    {
        private readonly ICandleshistoryservice _candlesHistoryService;
        private readonly IAlgoTradesClient _tradesHistoryService;
        private readonly IHistoryClient _historyService;
        private readonly IAlgoInstancesService _algoInstancesService;

        private readonly IAssetPairsReadModelRepository _assetPairsReadModel;
        private readonly AssetsValidator _assetsValidator;

        public AlgoInstanceHistoryService(ICandleshistoryservice candlesHistoryService,
                                          IAlgoTradesClient tradesHistoryService,
                                          IHistoryClient historyService,
                                          IAlgoInstancesService algoInstancesService,
                                          ILog log, IAssetPairsReadModelRepository assetPairsReadModel,
                                          [NotNull] AssetsValidator assetsValidator)
                                            : base(log, nameof(AlgoInstanceHistoryService))
        {
            this._candlesHistoryService = candlesHistoryService;
            this._tradesHistoryService = tradesHistoryService;
            this._historyService = historyService;
            _algoInstancesService = algoInstancesService;
            _assetPairsReadModel = assetPairsReadModel;
            _assetsValidator = assetsValidator;
        }

        public async Task<AlgoInstanceTradeResponse> GetTradesAsync(string instanceId, string tradedAssetId, DateTime fromMoment, DateTime toMoment/*, ModelStateDictionary errorsDictionary*/)
        {
            var result = await _tradesHistoryService.GetAlgoInstanceTradesByPeriod(instanceId, tradedAssetId, fromMoment, toMoment);
            return result;
        }

        public async Task<IEnumerable<QuoteChartingUpdate>> GetQuotesAsync(string instanceId, string assetPair, DateTime fromMoment, DateTime toMoment, bool? isBuy, string clientId,  ModelStateDictionary errorsDictionary)
        {
            var authToken = await GetAuthToken(instanceId, clientId, errorsDictionary);
            if (String.IsNullOrWhiteSpace(authToken))
                return null;

            var quotes = await _historyService.GetQuotes(fromMoment, toMoment, assetPair, instanceId, authToken, isBuy);

            if (quotes == null)
            {
                errorsDictionary.AddModelError("ServiceError", "Unknown");
                return null;
            }

            var result = quotes.Select(AutoMapper.Mapper.Map<Lykke.AlgoStore.Algo.Charting.QuoteChartingUpdate>);

            return result;
        }

        public string GetAssetPairName(string assetPairId)
        {
            var assetPairResponse = _assetPairsReadModel.TryGet(assetPairId);
            _assetsValidator.ValidateAssetPair(assetPairId, assetPairResponse);

            return assetPairResponse.Name;
        }

        private async Task<string> GetAuthToken(string instanceId, string clientId, ModelStateDictionary errorsDictionary)
        {
            var data = await _algoInstancesService.GetAlgoInstanceDataAsync(clientId, instanceId);
            if (String.IsNullOrEmpty(data?.AuthToken))
            {
                errorsDictionary.AddModelError("instanceId", "Invalid or not found");
                await Log.WriteWarningAsync(nameof(AlgoInstanceHistoryService), nameof(GetFunctionsAsync), $"AuthToken not found for clientId {clientId} and instanceId {instanceId}");
                return null;
            }
            return data.AuthToken;
        }

        public async Task<IEnumerable<FunctionChartingUpdate>> GetFunctionsAsync(string instanceId, DateTime fromMoment, DateTime toMoment, string clientId, ModelStateDictionary errorsDictionary)
        {
            var authToken = await GetAuthToken(instanceId, clientId, errorsDictionary);
            if (String.IsNullOrWhiteSpace(authToken))
                return null;

            var functions = await _historyService.GetFunctionValues(instanceId, fromMoment, toMoment, authToken);

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

            var httpResponse = await _candlesHistoryService.GetCandlesHistoryOrErrorWithHttpMessagesAsync(assetPair, CandleDataTypeMapper.PriceType()[priceType], CandleDataTypeMapper.TimeInterval()[timeInterval], fromMoment, toMoment, null, ctoken);

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
