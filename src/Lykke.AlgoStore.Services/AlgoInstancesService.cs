using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Services.Strings;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CandlesHistory.Client;
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
        private readonly IAssetsService _assetService;
        private readonly IClientAccountClient _clientAccountService;
        private readonly ICandleshistoryservice _candlesHistoryService;
        private readonly AssetsValidator _assetsValidator;
        private readonly IWalletBalanceService _walletBalanceService;

        public AlgoInstancesService(IAlgoRepository algoRepository,
            IAlgoClientInstanceRepository instanceRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IStatisticsRepository statisticsRepository,
            IAssetsService assetService,
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

                if (string.IsNullOrWhiteSpace(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");
                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId Is empty");

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

                return await _instanceRepository.GetAlgoInstanceDataByClientIdAsync(data.ClientId, data.InstanceId);
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
        public async Task<AlgoClientInstanceData> SaveAlgoBackTestInstanceDataAsync(AlgoClientInstanceData data, string algoClientId)
        {
            return await LogTimedInfoAsync(nameof(SaveAlgoBackTestInstanceDataAsync), data.ClientId,
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

                if (!await _algoRepository.ExistsAlgoAsync(data.AlgoClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Algo metadata not found for {data.AlgoId}");

                var result = await _instanceRepository.GetAlgoInstanceDataByClientIdAsync(data.ClientId, data.InstanceId);
                if (result == null || result.AlgoId == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound,
                        $"Algo instance data not found for client with id ${data.ClientId} and instanceId {data.InstanceId}");

                if (!result.ValidateData(out var instanceException))
                    throw instanceException;

                return result;
            });
        }

        private async Task<AlgoClientInstanceData> SaveInstanceDataAsync(
           AlgoClientInstanceData data,
           string algoClientId,
           bool isBackTestInstance = false)
        {
            if (string.IsNullOrWhiteSpace(data.InstanceId))
                data.InstanceId = Guid.NewGuid().ToString();

            if (!data.ValidateData(out var exception))
                throw exception;

            if (!isBackTestInstance)
            {
                var wallet = await GetClientWallet(data.ClientId, data.WalletId);
                if (wallet == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.WalletNotFound,
                        $"Wallet {data.WalletId} not found for client {data.ClientId}");

                if (!string.IsNullOrEmpty(data.WalletId) && await IsWalletUsedByExistingStartedInstance(data.WalletId))
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.WalletIsAlreadyUsed,
                        string.Format(Phrases.WalletIsAlreadyUsed, data.WalletId, data.AlgoId, data.ClientId),
                        Phrases.WalletAlreadyUsed);
                }
            }

            if (!await _algoRepository.ExistsAlgoAsync(algoClientId, data.AlgoId))
                throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                    $"Algo {data.AlgoId} no found for client {data.ClientId}");

            if (algoClientId != data.ClientId && !await _publicAlgosRepository.ExistsPublicAlgoAsync(algoClientId, data.AlgoId))
                throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotPublic,
                    $"Algo {data.AlgoId} not public for client {data.ClientId}");

            var assetPairResponse = await _assetService.AssetPairGetWithHttpMessagesAsync(data.AssetPair);
            _assetsValidator.ValidateAssetPairResponse(assetPairResponse);
            _assetsValidator.ValidateAssetPair(data.AssetPair, assetPairResponse.Body);

            var baseAsset = await _assetService.AssetGetWithHttpMessagesAsync(assetPairResponse.Body.BaseAssetId);
            _assetsValidator.ValidateAssetResponse(baseAsset);

            var quotingAsset = await _assetService.AssetGetWithHttpMessagesAsync(assetPairResponse.Body.QuotingAssetId);
            _assetsValidator.ValidateAssetResponse(quotingAsset);
            _assetsValidator.ValidateAsset(assetPairResponse.Body, data.TradedAsset, baseAsset.Body, quotingAsset.Body);

            var straight = data.TradedAsset == baseAsset.Body.Id || data.TradedAsset == baseAsset.Body.Name;

            //get traded asset
            var asset = straight ? baseAsset : quotingAsset;

            _assetsValidator.ValidateAccuracy(data.Volume, asset.Body.Accuracy);

            var volume = data.Volume.TruncateDecimalPlaces(asset.Body.Accuracy);
            var minVolume = straight ? assetPairResponse.Body.MinVolume : assetPairResponse.Body.MinInvertedVolume;
            _assetsValidator.ValidateVolume(volume, minVolume, asset.Body.DisplayId);

            if (!isBackTestInstance)
                _walletBalanceService.ValidateWallet(data.WalletId, assetPairResponse.Body);

            if (string.IsNullOrEmpty(data.AuthToken))
                data.AuthToken = Guid.NewGuid().ToString();

            data.IsStraight = straight;
            data.OppositeAssetId = straight ? quotingAsset.Body.Id : baseAsset.Body.Id;
            await _instanceRepository.SaveAlgoInstanceDataAsync(data);

            var res = await _instanceRepository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId);
            if (res == null)
                throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                    $"Cannot save {(isBackTestInstance ? "back test" : "")} algo instance data for {data.ClientId} id: {data.AlgoId}");

            await SaveSummaryStatistic(data, assetPairResponse.Body, asset.Body, straight ? quotingAsset.Body : baseAsset.Body);

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

            if (data.AlgoInstanceType == CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceType.Test)
            {
                userCurrencyAssetId = assetPair.QuotingAssetId;

                clientTradedAssetBalance = data.BackTestTradingAssetBalance;
                clientAssetTwoBalance = data.BackTestAssetTwoBalance;

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
                    initialWalletBalance = clientAssetTwoBalance + tradedAssetBalanceAbsoluteValue.History.First().Close * clientTradedAssetBalance;
                else
                    initialWalletBalance = clientTradedAssetBalance + tradedAssetBalanceAbsoluteValue.History.First().Close * clientAssetTwoBalance;

            }
            else
            {
                var baseUserAssetId = await GetBaseAssetAsync(data.ClientId);
                var assetResponse = await _assetService.AssetGetWithHttpMessagesAsync(baseUserAssetId.BaseAssetId);

                if (assetResponse == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"There is no asset with an id {baseUserAssetId.BaseAssetId}");

                userCurrencyAssetId = baseUserAssetId.BaseAssetId;

                var walletBalances = await _walletBalanceService.GetWalletBalancesAsync(data.WalletId, assetPair);
                initialWalletBalance = await _walletBalanceService.GetTotalWalletBalanceInBaseAssetAsync(data.WalletId, baseUserAssetId.BaseAssetId, assetPair);

                var clientBalanceResponseModels = walletBalances.ToList();
                clientTradedAssetBalance = clientBalanceResponseModels.First(b => b.AssetId == tradedAsset.Id).Balance;
                clientAssetTwoBalance = clientBalanceResponseModels.First(b => b.AssetId != tradedAsset.Id).Balance;
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
                    $"Base asset for user {clientId} not found");
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
            var algoInstances = (await _instanceRepository.GetAllByWalletIdAndInstanceStatusIsNotStoppedAsync(walletId));
            return algoInstances != null && algoInstances.Any();
        }
    }
}
