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
        private readonly IReloadingManager<string> _connectionStringManager; //i think we dont need this
        private readonly ILog _log;

        public AlgoRuntimeDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _log = log;
            _connectionStringManager = connectionStringManager;
            _table = AzureTableStorage<AlgoRuntimeDataEntity>.Create(connectionStringManager, TableName, _log); 
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

        public async Task SaveAlgoRuntimeData(AlgoClientRuntimeData data)
        {
            var enitites = data.ToEntity(PartitionKey);

            await _table.InsertOrMergeBatchAsync(enitites);
        }

        public async Task<bool> DeleteAlgoRuntimeData(string imageId)
        {
            var entity = await _table.DeleteAsync(PartitionKey, imageId);
            return entity != null;
        }
    }
}
