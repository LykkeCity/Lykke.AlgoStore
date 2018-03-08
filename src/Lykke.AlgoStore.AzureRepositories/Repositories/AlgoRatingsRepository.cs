using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoRatingsRepository : IAlgoRatingsRepository
    {
        private static readonly Random Rnd = new Random();
        public static readonly string TableName = "AlgoRatingsTable";

        private readonly INoSQLTableStorage<AlgoRatingEntity> _table;

        public AlgoRatingsRepository(INoSQLTableStorage<AlgoRatingEntity> table)
        {
            _table = table;
        }

        public async Task<AlgoRatingData> GetAlgoRatingForClientAsync(string clientId, string algoId)
        {
            var result = await _table.GetDataAsync(clientId, algoId);
            return result.ToModel();
        }

        public async Task<List<AlgoRatingData>> GetAlgoRatingAsync(string algoId)
        {
            var result = await _table.GetDataAsync(algoId);
            return result.ToList().ToModel();
        }

        public async Task SaveAlgoRatingAsync(AlgoRatingData data)
        {
            var entities = data.ToEntity();
            await _table.InsertOrReplaceAsync(entities);
        }
    }
}
