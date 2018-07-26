using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Services.Strings;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.CandlesHistory.Client.Models;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.Models;

namespace Lykke.AlgoStore.Services
{
    public class AlgoInstancesService : BaseAlgoStoreService, IAlgoInstancesService
    {
        private readonly IAlgoRepository _algoRepository;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly IPublicAlgosRepository _publicAlgosRepository;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IAssetsServiceWithCache _assetService;
        private readonly IClientAccountClient _clientAccountService;
        private readonly ICandleshistoryservice _candlesHistoryService;
        private readonly AssetsValidator _assetsValidator;
        private readonly IWalletBalanceService _walletBalanceService;

        public AlgoInstancesService(IAlgoRepository algoRepository,
            IAlgoClientInstanceRepository instanceRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IStatisticsRepository statisticsRepository,
            IAssetsServiceWithCache assetService,
            IClientAccountClient clientAccountClient,
            ICandleshistoryservice candlesHistoryService,
            [NotNull] AssetsValidator assetsValidator,
            IWalletBalanceService walletBalanceService,

            ILog log
            ) : base(log, nameof(AlgoInstancesService))
        {
            _algoRepository = algoRepository;
            _instanceRepository = instanceRepository;
            _publicAlgosRepository = publicAlgosRepository;
            _statisticsRepository = statisticsRepository;
            _assetService = assetService;
            _clientAccountService = clientAccountClient;
            _candlesHistoryService = candlesHistoryService;
            _assetsValidator = assetsValidator;
            _walletBalanceService = walletBalanceService;
        }

        /// <summary>
        /// Gets all algo instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataByAlgoIdAndClientIdAsync(CSharp.AlgoTemplate.Models.Models.BaseAlgoData data)
        {
            return await LogTimedInfoAsync(nameof(GetAllAlgoInstanceDataByAlgoIdAndClientIdAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                Check.IsEmpty(data.ClientId, nameof(data.ClientId));
                Check.IsEmpty(data.AlgoId, nameof(data.AlgoId));

                var result = await _instanceRepository.GetAllAlgoInstancesByAlgoIdAndClienIdAsync(data.AlgoId, data.ClientId);

                return result.ToList();
            });
        }

        /// <summary>
        /// Gets the algo instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(BaseAlgoInstance data)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoInstanceDataAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoInstanceData = await _instanceRepository.GetAlgoInstanceDataByClientIdAsync(data.ClientId, data.InstanceId);
                var algo = await _algoRepository.GetAlgoDataInformationAsync(algoInstanceData.AlgoClientId, algoInstanceData.AlgoId);

                foreach (var param in algoInstanceData.AlgoMetaDataInformation.Parameters)
                {
                    param.PredefinedValues = algo.AlgoMetaDataInformation
                                                 .Parameters
                                                 .FirstOrDefault(p => p.Key == param.Key)
                                                 ?.PredefinedValues ?? new List<EnumValue>();
                }

                foreach (var function in algoInstanceData.AlgoMetaDataInformation.Functions)
                {
                    var algoFunction = algo.AlgoMetaDataInformation.Functions.FirstOrDefault(f => f.Id == function.Id);
                    if (algoFunction == null) continue;

                    foreach (var fParam in function.Parameters)
                    {
                        fParam.PredefinedValues = algoFunction.Parameters
                                                              .FirstOrDefault(p => p.Key == fParam.Key)
                                                              ?.PredefinedValues ?? new List<EnumValue>();
                    }
                }

                return algoInstanceData;
            });
        }

        /// <summary>
        /// Gets the algo instance data by ClientId asynchronous.
        /// </summary>
        /// <param name="clientId">The Id of the user.</param>
        /// <param name="instanceId">The Id of the algo instance.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(string clientId, string instanceId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoInstanceDataAsync), clientId, async () =>
                await _instanceRepository.GetAlgoInstanceDataByClientIdAsync(clientId, instanceId));
        }

        /// <summary>
        /// Saves the algo instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="algoClientId">Algo client id.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> SaveAlgoInstanceDataAsync(AlgoClientInstanceData data, string algoClientId)
        {
            return await LogTimedInfoAsync(nameof(SaveAlgoInstanceDataAsync), data.ClientId,
                async () => await SaveInstanceDataAsync(data, algoClientId));
        }

        /// <summary>
        /// Saves the algo back-test instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="algoClientId">Algo client id.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> SaveAlgoFakeTradingInstanceDataAsync(AlgoClientInstanceData data, string algoClientId)
        {
            return await LogTimedInfoAsync(nameof(SaveAlgoFakeTradingInstanceDataAsync), data.ClientId,
                async () => await SaveInstanceDataAsync(data, algoClientId, true));
        }

        /// <summary>
        /// Validates the cascade delete client metadata request asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> ValidateCascadeDeleteClientMetadataRequestAsync(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(ValidateCascadeDeleteClientMetadataRequestAsync), data?.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                await Check.Algo.Exists(_algoRepository, data.AlgoClientId, data.AlgoId);

                var result = await _instanceRepository.GetAlgoInstanceDataByClientIdAsync(data.ClientId, data.InstanceId);
                if (result == null || result.AlgoId == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound,
                        $"Algo instance data not found for client with id ${data.ClientId} and instanceId {data.InstanceId}",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "algo instance"));

                if (!result.ValidateData(out var instanceException))
                    throw instanceException;

                return result;
            });
        }

        private async Task<AlgoClientInstanceData> SaveInstanceDataAsync(
            AlgoClientInstanceData data,
            string algoClientId,
            bool isFakeTradeInstance = false)
        {
            if (string.IsNullOrWhiteSpace(data.InstanceId))
                data.InstanceId = Guid.NewGuid().ToString();

            if (!data.ValidateData(out var exception))
                throw exception;

            ValidateInstanceMetadataDates(data.AlgoMetaDataInformation);

            if (isFakeTradeInstance &&
                data.AlgoInstanceType == CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceType.Live)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                    Phrases.LiveAlgoCantFakeTrade,
                    Phrases.LiveAlgoCantFakeTrade);
            }
            else if (!isFakeTradeInstance &&
                     data.AlgoInstanceType != CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceType.Live)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                    Phrases.DemoOrBacktestCantRunLive,
                    Phrases.DemoOrBacktestCantRunLive);
            }

            if (!isFakeTradeInstance)
            {
                var wallet = await GetClientWallet(data.ClientId, data.WalletId);
                if (wallet == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.WalletNotFound,
                        $"Wallet {data.WalletId} not found for client {data.ClientId}",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "wallet"));

                if (!string.IsNullOrEmpty(data.WalletId) && await IsWalletUsedByExistingStartedInstance(data.WalletId))
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.WalletIsAlreadyUsed,
                        string.Format(Phrases.WalletIsAlreadyUsed, data.WalletId, data.AlgoId, data.ClientId),
                        Phrases.WalletAlreadyUsed);
                }
            }

            await Check.Algo.Exists(_algoRepository, algoClientId, data.AlgoId);
            await Check.Algo.IsVisibleForClient(_publicAlgosRepository, data.AlgoId, data.ClientId, algoClientId);

            var assetPairResponse = await _assetService.TryGetAssetPairAsync(data.AssetPairId);
            _assetsValidator.ValidateAssetPair(data.AssetPairId, assetPairResponse);

            var baseAsset = await _assetService.TryGetAssetAsync(assetPairResponse.BaseAssetId);
            _assetsValidator.ValidateAssetResponse(baseAsset);

            var quotingAsset = await _assetService.TryGetAssetAsync(assetPairResponse.QuotingAssetId);
            _assetsValidator.ValidateAssetResponse(quotingAsset);
            _assetsValidator.ValidateAsset(assetPairResponse, data.TradedAssetId, baseAsset, quotingAsset);

            var straight = data.TradedAssetId == baseAsset.Id || data.TradedAssetId == baseAsset.Name;

            //get traded asset
            var tradedAsset = straight ? baseAsset : quotingAsset;

            _assetsValidator.ValidateAccuracy(data.Volume, tradedAsset.Accuracy);

            var volume = data.Volume.TruncateDecimalPlaces(tradedAsset.Accuracy);
            var minVolume = straight ? assetPairResponse.MinVolume : assetPairResponse.MinInvertedVolume;
            _assetsValidator.ValidateVolume(volume, minVolume, tradedAsset.DisplayId);

            if (!isFakeTradeInstance)
                _walletBalanceService.ValidateWallet(data.WalletId, assetPairResponse);

            if (string.IsNullOrEmpty(data.AuthToken))
                data.AuthToken = Guid.NewGuid().ToString();

            data.IsStraight = straight;
            data.OppositeAssetId = straight ? quotingAsset.Id : baseAsset.Id;
            data.AlgoInstanceCreateDate = DateTime.UtcNow;

            await SaveSummaryStatistic(data, assetPairResponse, tradedAsset, straight ? quotingAsset : baseAsset);

            await _instanceRepository.SaveAlgoInstanceDataAsync(data);

            var res = await _instanceRepository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId);
            if (res == null)
            {
                await _statisticsRepository.DeleteSummaryAsync(data.InstanceId);

                throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                    $"Cannot save {(isFakeTradeInstance ? "back test" : "")} algo instance data with insatnce id: {data.InstanceId} for client id: {data.ClientId} and algo id: {data.AlgoId}",
                    string.Format(Phrases.ParamNotFoundDisplayMessage, "algo instance"));
            }

            return res;
        }

        /// <summary>
        /// Create statistics summary and save initial wallet status in it
        /// </summary>
        /// <param name="data">The algo instance data</param>
        /// <param name="assetPair">The asset pair for the algo instance</param>
        /// <param name="tradedAsset">The traded asset from the Asset pair</param>
        /// <param name="assetTwo">The second asset from the Asset pair</param>
        private async Task SaveSummaryStatistic(AlgoClientInstanceData data, AssetPair assetPair, Asset tradedAsset, Asset assetTwo)
        {
            double clientTradedAssetBalance;
            double clientAssetTwoBalance;
            double initialWalletBalance;
            string userCurrencyAssetId;

            if (data.AlgoInstanceType != CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceType.Live)
            {
                userCurrencyAssetId = assetPair.QuotingAssetId;

                clientTradedAssetBalance = data.FakeTradingTradingAssetBalance;
                clientAssetTwoBalance = data.FakeTradingAssetTwoBalance;

                var tradedAssetBalanceAbsoluteValue = await _candlesHistoryService.GetCandlesHistoryAsync(assetPair.Id,
                    Lykke.Service.CandlesHistory.Client.Models.CandlePriceType.Mid,
                    Lykke.Service.CandlesHistory.Client.Models.CandleTimeInterval.Day,
                    DateTime.Parse(data.AlgoMetaDataInformation.Parameters.First(p => p.Key == "StartFrom").Value),
                    DateTime.Parse(data.AlgoMetaDataInformation.Parameters.First(p => p.Key == "StartFrom").Value));

                if (!tradedAssetBalanceAbsoluteValue.History.Any())
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InitialWalletBalanceNotCalculated,
                        $"Initial wallet balance could not be calculated. Could not get history price for {assetPair.Name}");
                }
                //show balance for the quoting asset from the Asset pair - for back test
                if (data.IsStraight)
                    initialWalletBalance = clientAssetTwoBalance + (tradedAssetBalanceAbsoluteValue?.History.First().Close ?? 0) * clientTradedAssetBalance;
                else
                    initialWalletBalance = clientTradedAssetBalance + (tradedAssetBalanceAbsoluteValue?.History.First().Close ?? 0) * clientAssetTwoBalance;
            }
            else
            {
                var baseUserAssetId = await GetBaseAssetAsync(data.ClientId);
                var assetResponse = await _assetService.TryGetAssetAsync(baseUserAssetId.BaseAssetId);

                if (assetResponse == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.NotFound, $"There is no asset with an id {baseUserAssetId.BaseAssetId}",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "base user asset"));

                userCurrencyAssetId = baseUserAssetId.BaseAssetId;

                var walletBalances = await _walletBalanceService.GetWalletBalancesAsync(data.WalletId, assetPair);
                initialWalletBalance = await _walletBalanceService.GetTotalWalletBalanceInBaseAssetAsync(data.WalletId, baseUserAssetId.BaseAssetId, assetPair);

                var clientBalanceResponseModels = walletBalances.ToList();
                clientTradedAssetBalance = clientBalanceResponseModels.FirstOrDefault(b => b.AssetId == tradedAsset.Id)?.Balance ?? 0;
                clientAssetTwoBalance = clientBalanceResponseModels.FirstOrDefault(b => b.AssetId != tradedAsset.Id)?.Balance ?? 0;
            }

            await _statisticsRepository.CreateOrUpdateSummaryAsync(new StatisticsSummary
            {
                InitialWalletBalance = initialWalletBalance,
                InitialTradedAssetBalance = clientTradedAssetBalance,
                InitialAssetTwoBalance = clientAssetTwoBalance,
                LastTradedAssetBalance = clientTradedAssetBalance,
                LastAssetTwoBalance = clientAssetTwoBalance,
                TradedAssetName = tradedAsset.Name,
                AssetTwoName = assetTwo.Name,
                InstanceId = data.InstanceId,
                LastWalletBalance = initialWalletBalance,
                TotalNumberOfStarts = 0,
                TotalNumberOfTrades = 0,
                UserCurrencyBaseAssetId = userCurrencyAssetId
            });

            var statisticsSummaryResult = await _statisticsRepository.GetSummaryAsync(data.InstanceId);
            if (statisticsSummaryResult == null)
                throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                    $"Could not save summary row for AlgoInstance: {data.InstanceId}, User: {data.ClientId} AlgoId: {data.AlgoId}");
        }

        private async Task<WalletDtoModel> GetClientWallet(string clientId, string walletId)
        {
            var wallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);
            return wallets?.FirstOrDefault(x => x.Id == walletId);
        }

        /// <summary>
        /// Get the base asset Id for the user
        /// </summary>
        /// <param name="clientId">User Id</param>
        /// <returns></returns>
        private async Task<BaseAssetClientModel> GetBaseAssetAsync(string clientId)
        {
            var baseAsset = await _clientAccountService.GetBaseAssetAsync(clientId);
            if (baseAsset == null)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.AssetNotFound,
                    $"Base asset for user {clientId} not found",
                    string.Format(Phrases.ParamNotFoundDisplayMessage, "asset"));
            }

            return baseAsset;
        }

        /// <summary>
        /// Check if the wallet is used by another running algo instance of the user.
        /// </summary>
        /// <param name="walletId">
        /// Wallet id that the user wants to use for trading
        /// </param>
        private async Task<bool> IsWalletUsedByExistingStartedInstance(string walletId)
        {
            var algoInstances = await _instanceRepository.GetAllByWalletIdAndInstanceStatusIsNotStoppedAsync(walletId);
            return algoInstances != null && algoInstances.Any();
        }

        private void ValidateInstanceMetadataDates(AlgoMetaDataInformation instanceMetadata)
        {
            var dtType = typeof(DateTime).FullName;

            var instanceParameters = instanceMetadata.Parameters.Where(p => p.Type == dtType).ToList();
            var startFromDate = instanceParameters.SingleOrDefault(t => t.Key == "StartFrom")?.Value;
            var endOnDate = instanceParameters.SingleOrDefault(t => t.Key == "EndOn")?.Value;

            var instanceStartFromDate = DateTime.ParseExact(startFromDate, AlgoStoreConstants.DateTimeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal);

            var instanceEndOnDateDate = DateTime.ParseExact(endOnDate, AlgoStoreConstants.DateTimeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal);

            if (instanceStartFromDate >= instanceEndOnDateDate)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                    "StartFrom date cannot be later than or equal to EndOn date",
                    string.Format(Phrases.DatesValidationMessage, "Algo"));
            }

            foreach (var function in instanceMetadata.Functions)
            {
                var functionStartingDateString = function.Parameters.Where(p => p.Type == dtType)
                    .SingleOrDefault(t => t.Key == "startingDate")?.Value;

                var functionEndingDateString = function.Parameters.Where(p => p.Type == dtType)
                    .SingleOrDefault(t => t.Key == "endingDate")?.Value;

                if (string.IsNullOrEmpty(functionStartingDateString) || string.IsNullOrEmpty(functionEndingDateString))
                    continue;

                var functionStartingDate = DateTime.ParseExact(functionStartingDateString, AlgoStoreConstants.DateTimeFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal);

                var functionEndingDate = DateTime.ParseExact(functionEndingDateString, AlgoStoreConstants.DateTimeFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal);

                if (functionStartingDate >= functionEndingDate)
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        "StartFrom date cannot be later than or equal to EndOn date",
                        string.Format(Phrases.DatesValidationMessage, "Algo Function"));
                }
            }
        }

        public async Task<List<UserInstanceData>> GetUserInstancesAsync(string clientId)
        {
            return await LogTimedInfoAsync(nameof(ValidateCascadeDeleteClientMetadataRequestAsync), clientId, async () =>
            {
                var instances = await _instanceRepository.GetAllAlgoInstancesByClientAsync(clientId);
                var wallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);
                var walletData = wallets.Select(w => new ClientWalletData()
                {
                    Id = w.Id,
                    Name = w.Name
                });

                var result = instances.Select(i => new UserInstanceData()
                {
                    InstanceId = i.InstanceId,
                    InstanceName = i.InstanceName,
                    AlgoClientId = i.AlgoClientId,
                    AlgoId = i.AlgoId,
                    CreateDate = i.AlgoInstanceCreateDate,
                    RunDate = i.AlgoInstanceRunDate,
                    StopDate = i.AlgoInstanceStopDate,
                    InstanceType = i.AlgoInstanceType,
                    InstanceStatus = i.AlgoInstanceStatus,
                    Wallet = walletData.FirstOrDefault(w => w.Id == i.WalletId)
                }).ToList();

                return result;
            });
        }

        public async Task ValidateAlgoInstancesDeploymentLimits(string clientId)
        {
            await Check.AlgoInstance.InstancesOverDeploymentLimit(_instanceRepository, clientId);
        }
    }
}
