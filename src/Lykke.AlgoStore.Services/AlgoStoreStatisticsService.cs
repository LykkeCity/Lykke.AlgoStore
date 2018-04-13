﻿using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.Service.Assets.Client;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreStatisticsService : BaseAlgoStoreService, IAlgoStoreStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IAlgoClientInstanceRepository _algoInstanceRepository;
        private readonly IWalletBalanceService _walletBalanceService;
        private readonly IAssetsService _assetService;

        public AlgoStoreStatisticsService(IStatisticsRepository statisticsRepository, IAlgoClientInstanceRepository algoClientInstanceRepository,
            IWalletBalanceService walletBalanceService, IAssetsService assetsService, ILog log) : base(log,
            nameof(AlgoStoreStatisticsService))
        {
            _statisticsRepository = statisticsRepository;
            _algoInstanceRepository = algoClientInstanceRepository;
            _walletBalanceService = walletBalanceService;
            _assetService = assetsService;
        }

        public async Task<StatisticsSummary> GetStatisticsSummaryAsync(string clientId, string instanceId)
        {
            return await LogTimedInfoAsync(
                nameof(GetStatisticsSummaryAsync),
                clientId,
                async () =>
                {
                    if (string.IsNullOrEmpty(instanceId))
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "InstanceId is empty.");

                    var statisticsSummary = await _statisticsRepository.GetSummaryAsync(instanceId);
                    if (statisticsSummary == null)
                    {
                        throw new AlgoStoreException(AlgoStoreErrorCodes.StatisticsSumaryNotFound,
                            $"Could not find statistic summary row for AlgoInstance: {instanceId}");
                    }

                    return statisticsSummary;
                }
            );
        }

        public async Task<StatisticsSummary> UpdateStatisticsSummaryAsync(string clientId, string instanceId)
        {
            return await LogTimedInfoAsync(
                nameof(UpdateStatisticsSummaryAsync),
                clientId,
                async () =>
                {
                    if (string.IsNullOrEmpty(instanceId))
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "InstanceId is empty.");

                    var statisticsSummary = await _statisticsRepository.GetSummaryAsync(instanceId);
                    if (statisticsSummary == null)
                    {
                        throw new AlgoStoreException(AlgoStoreErrorCodes.StatisticsSumaryNotFound,
                            $"Could not find statistic summary row for AlgoInstance: {instanceId}");
                    }

                    var algoInstance = await _algoInstanceRepository.GetAlgoInstanceDataByClientIdAsync(clientId, instanceId);
                    if (algoInstance == null)
                    {
                        throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound,
                            $"Could not find AlgoInstance: {instanceId}");
                    }

                    var assetPairResponse = await _assetService.AssetPairGetWithHttpMessagesAsync(algoInstance.AssetPair);

                    var walletBalances = await _walletBalanceService.GetWalletBalancesAsync(algoInstance.WalletId, assetPairResponse.Body);
                    var clientBalanceResponseModels = walletBalances.ToList();
                    var latestWalletBalance = await _walletBalanceService.GetTotalWalletBalanceInBaseAssetAsync(
                        algoInstance.WalletId, statisticsSummary.UserCurrencyBaseAssetId, assetPairResponse.Body);

                    statisticsSummary.LastTradedAssetBalance = clientBalanceResponseModels.First(b => b.AssetId == algoInstance.TradedAsset).Balance;
                    statisticsSummary.LastAssetTwoBalance = clientBalanceResponseModels.First(b => b.AssetId != algoInstance.TradedAsset).Balance;
                    statisticsSummary.LastWalletBalance = latestWalletBalance;
                    statisticsSummary.NetProfit = ((statisticsSummary.LastWalletBalance - statisticsSummary.InitialWalletBalance) /
                                       statisticsSummary.InitialWalletBalance) * 100;

                    await _statisticsRepository.CreateOrUpdateSummaryAsync(statisticsSummary);

                    return statisticsSummary;
                }
            );
        }
    }
}
