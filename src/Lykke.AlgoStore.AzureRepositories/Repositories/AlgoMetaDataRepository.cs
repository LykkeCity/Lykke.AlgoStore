using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoMetaDataRepository : IAlgoMetaDataRepository
    {
        public static readonly string TableName = "AlgoMetaDataTable";

        private readonly INoSQLTableStorage<AlgoMetaDataEntity> _table;

        public AlgoMetaDataRepository(INoSQLTableStorage<AlgoMetaDataEntity> table)
        {
            _table = table;
        }

        public async Task<AlgoClientMetaData> GetAllClientAlgoMetaData(string clientId)
        {
            var entities = await _table.GetDataAsync(clientId);

            var result = entities.ToList().ToModel();

            if (!result.AlgoMetaData.IsNullOrEmptyCollection())
            {
                result.AlgoMetaData.Sort();
                result.AlgoMetaData.Reverse();
            }

            return result;
        }
        public async Task<AlgoClientMetaData> GetAlgoMetaData(string clientId, string algoId)
        {
            var entitiy = await _table.GetDataAsync(clientId, algoId);

            return new[] { entitiy }.ToModel();
        }
        public async Task<bool> ExistsAlgoMetaData(string clientId, string algoId)
        {
            var entity = new AlgoMetaDataEntity();
            entity.PartitionKey = clientId;
            entity.RowKey = algoId;

            return await _table.RecordExistsAsync(entity);
        }

        public async Task SaveAlgoMetaData(AlgoClientMetaData metaData)
        {
            var enitites = metaData.ToEntity();

            await _table.InsertOrMergeBatchAsync(enitites);
        }
        public async Task DeleteAlgoMetaData(AlgoClientMetaData metaData)
        {
            var entities = metaData.ToEntity();
            await _table.DeleteAsync(entities);
        }
    }
}
