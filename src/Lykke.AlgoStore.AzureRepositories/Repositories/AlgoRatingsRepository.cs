using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoRatingsRepository : IAlgoRatingsRepository
    {
        public static readonly string TableName = "AlgoRatingsTable";

        private readonly INoSQLTableStorage<AlgoRatingEntity> _table;

        public AlgoRatingsRepository(INoSQLTableStorage<AlgoRatingEntity> table)
        {
            _table = table;
        }

        public async Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId)
        {
            var result = await _table.GetDataAsync(algoId, clientId);
            return result.ToModel();
        }

        public async Task<List<AlgoRatingData>> GetAlgoRatingsAsync(string algoId)
        {
            var result = await _table.GetDataAsync(algoId);
            return result.ToList().ToModel();
        }

        public async Task SaveAlgoRatingAsync(AlgoRatingData data)
        {
            var entities = data.ToEntity();
            await _table.InsertOrReplaceAsync(entities);
        }

        public async Task DeleteRatingsAsync(string algoId)
        {
            var query = new TableQuery<AlgoRatingEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, algoId))
                .Take(100);

            await _table.ExecuteAsync(query, async (ratings) =>
            {
                var tableBatchOperation = new TableBatchOperation();

                foreach(var rating in ratings)
                {
                    tableBatchOperation.Delete(rating);
                }

                await _table.DoBatchAsync(tableBatchOperation);
            }, () => true);
        }
    }
}
