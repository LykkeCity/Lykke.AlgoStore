using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Utils;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoMetaDataRepository : IAlgoMetaDataRepository
    {
        private const string PartitionKey = "AlgoMetaData";
        private const string TableName = "AlgoMetaDataTable";

        private readonly INoSQLTableStorage<AlgoMetaDataEntity> _table;

        public AlgoMetaDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _table = AzureTableStorage<AlgoMetaDataEntity>.Create(connectionStringManager, TableName, log);
        }

        public async Task<AlgoClientMetaData> GetAllClientAlgoMetaData(string clientId)
        {
            var entities = await _table.GetDataAsync(PartitionKey, data => data.ClientId == clientId);

            var result = entities.ToModel();

            if (!result.AlgoMetaData.IsNullOrEmptyCollection())
            {
                result.AlgoMetaData.Sort();
                result.AlgoMetaData.Reverse();
            }

            return result;
        }
        public async Task<AlgoClientMetaData> GetAlgoMetaData(string id)
        {
            var entitiy = await _table.GetDataAsync(PartitionKey, id);

            return new AlgoMetaDataEntity[1] { entitiy }.ToModel();
        }
        public async Task SaveAlgoMetaData(AlgoClientMetaData metaData)
        {
            var enitites = metaData.ToEntity(PartitionKey);

            await _table.InsertOrMergeBatchAsync(enitites);
        }
        public async Task DeleteAlgoMetaData(AlgoClientMetaData metaData)
        {
            var entities = metaData.ToEntity(PartitionKey);
            await _table.DeleteAsync(entities);
        }
        public async Task<bool> ExistsAlgoMetaData(string id)
        {
            var entity = new AlgoMetaDataEntity();
            entity.PartitionKey = PartitionKey;
            entity.RowKey = id;

            return await _table.RecordExistsAsync(entity);
        }
    }
}
