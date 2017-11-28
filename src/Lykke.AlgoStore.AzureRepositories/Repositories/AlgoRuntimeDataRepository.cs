using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoRuntimeDataRepository : IAlgoRuntimeDataRepository
    {
        private const string PartitionKey = "AlgoRuntimeData";
        private const string TableName = "AlgoRuntimeDataTable";

        private readonly INoSQLTableStorage<AlgoRuntimeDataEntity> _table;

        public AlgoRuntimeDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _table = AzureTableStorage<AlgoRuntimeDataEntity>.Create(connectionStringManager, TableName, log);
        }

        public async Task<AlgoClientRuntimeData> GetAlgoRuntimeData(string imageId)
        {
            var entity = await _table.GetDataAsync(PartitionKey, imageId);

            return new AlgoRuntimeDataEntity[1] { entity }.ToModel();
        }

        public async Task<AlgoClientRuntimeData> GetAlgoRuntimeDataByAlgo(string algoId)
        {
            var entities = await _table.GetDataAsync(PartitionKey, entity => entity.ClientAlgoId == algoId);

            return entities.ToModel();
        }

        public async Task<AlgoClientRuntimeData> SaveAlgoRuntimeData(AlgoClientRuntimeData data)
        {
            var enitites = data.ToEntity(PartitionKey);

            await _table.InsertOrMergeBatchAsync(enitites);

            var keys = enitites.Select(e => e.RowKey).ToArray();

            var result = await _table.GetDataAsync(PartitionKey, keys);

            return result.ToModel();
        }

        public async Task<bool> DeleteAlgoRuntimeData(string imageId)
        {
            var entity = await _table.DeleteAsync(PartitionKey, imageId);
            return entity != null;
        }
    }
}
