using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreStatisticsService : BaseAlgoStoreService, IAlgoStoreStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public AlgoStoreStatisticsService(ILog log, IStatisticsRepository statisticsRepository) : base(log,
            nameof(AlgoStoreStatisticsService))
        {
            _statisticsRepository = statisticsRepository;
        }

        public async Task<StatisticsSummary> GetAlgoInstanceStatisticsAsync(string instanceId)
        {
            return await LogTimedInfoAsync(
                nameof(GetAlgoInstanceStatisticsAsync),
                instanceId,
                async () =>
                {
                    if (string.IsNullOrEmpty(instanceId))
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "InstanceId is empty.");

                    var result = await _statisticsRepository.GetSummaryAsync(instanceId);

                    return result;
                }
            );
        }
    }
}
