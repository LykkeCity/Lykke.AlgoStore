using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Statistics.Client;
using Lykke.AlgoStore.Services.Strings;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Service.Assets.Client;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreStatisticsService : BaseAlgoStoreService, IAlgoStoreStatisticsService
    {
        private readonly IStatisticsClient _statisticsClient;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IAlgoClientInstanceRepository _algoInstanceRepository;
        private readonly IWalletBalanceService _walletBalanceService;
        private readonly IAssetsServiceWithCache _assetService;
        private readonly AssetsValidator _assetsValidator;

        public AlgoStoreStatisticsService(IStatisticsRepository statisticsRepository,
            IAlgoClientInstanceRepository algoClientInstanceRepository,
            IWalletBalanceService walletBalanceService, 
            IAssetsServiceWithCache assetsService,
            [NotNull] AssetsValidator assetsValidator,
            IStatisticsClient statisticsClient,
            ILog log)
            : base(log, nameof(AlgoStoreStatisticsService))
        {
            _statisticsRepository = statisticsRepository;
            _algoInstanceRepository = algoClientInstanceRepository;
            _walletBalanceService = walletBalanceService;
            _assetService = assetsService;
            _statisticsClient = statisticsClient;
            _assetsValidator = assetsValidator;
        }

        //REMARK: In future we will MOVE this method into new statistics service (Lykke.AlgoStore.Statistics.Service solution)
        //When that is done we should reconsider if we need additional endpoint,
        //e.g. GetSummaryAsync that is doing same thing here and in method below
        //All of this will require us to MOVE everything related to statistics from shared Models project too
        public async Task<StatisticsSummary> GetStatisticsSummaryAsync(string clientId, string instanceId)
        {
            return await LogTimedInfoAsync(
                nameof(GetStatisticsSummaryAsync),
                clientId,
                async () =>
                {
                    Check.IsEmpty(instanceId, nameof(instanceId));

                    var statisticsSummary = await _statisticsRepository.GetSummaryAsync(instanceId);
                    if (statisticsSummary == null)
                    {
                        throw new AlgoStoreException(AlgoStoreErrorCodes.StatisticsSumaryNotFound,
                            $"Could not find statistic summary row for AlgoInstance: {instanceId}",
                            string.Format(Phrases.ParamNotFoundDisplayMessage, "statistics summary"));
                    }

                    statisticsSummary.NetProfit = statisticsSummary.InitialWalletBalance.Equals(0.0) ? 0 : Math.Round(
                         ((statisticsSummary.LastWalletBalance - statisticsSummary.InitialWalletBalance) /
                          statisticsSummary.InitialWalletBalance) * 100, 2, MidpointRounding.AwayFromZero);

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
                    Check.IsEmpty(instanceId, nameof(instanceId));

                    var statisticsSummary = await _statisticsRepository.GetSummaryAsync(instanceId);
                    if (statisticsSummary == null)
                    {
                        throw new AlgoStoreException(AlgoStoreErrorCodes.StatisticsSumaryNotFound,
                            $"Could not find statistic summary row for AlgoInstance: {instanceId}",
                            string.Format(Phrases.ParamNotFoundDisplayMessage, "statistics summary"));
                    }

                    await _statisticsClient.UpdateSummaryAsync(clientId, instanceId);

                    return statisticsSummary;
                }
            );
        }
    }
}
