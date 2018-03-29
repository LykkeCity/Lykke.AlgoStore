﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreTradesService : BaseAlgoStoreService, IAlgoStoreTradesService
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public AlgoStoreTradesService(ILog log, IStatisticsRepository statisticsRepository) : base(log, nameof(AlgoStoreTradesService))
        {
            _statisticsRepository = statisticsRepository;
        }

        public async Task<List<Statistics>> GetAllTradesForAlgoInstanceAsync(string instanceId)
        {
            return await LogTimedInfoAsync(nameof(GetAllTradesForAlgoInstanceAsync), instanceId, async () =>
            {
                if (string.IsNullOrEmpty(instanceId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "InstanceId is empty.");

                var result = await _statisticsRepository.GetAllStatisticsAsync(instanceId);

                return result;
            });
        }
    }
}
