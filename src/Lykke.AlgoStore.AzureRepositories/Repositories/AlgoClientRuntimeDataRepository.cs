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
    public class AlgoClientRuntimeDataRepository : IAlgoClientRuntimeDataRepository
    {
        private const string AlgoDataTableName = "AlgoRuntimeData";

        private readonly INoSQLTableStorage<AlgoRuntimeEntity> _table;
        private readonly IReloadingManager<string> _connectionStringManager;
        private readonly ILog _log;

        public AlgoClientRuntimeDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _log = log;
            _connectionStringManager = connectionStringManager;
            _table = AzureTableStorage<AlgoRuntimeEntity>.Create(connectionStringManager, AlgoDataTableName, _log);
        }

        public async Task<AlgoClientRuntimeData> GetAlgoRuntimeData(string clientId, string imageId)
        {
            var entity = await _table.GetDataAsync(clientId, imageId);

            return new AlgoRuntimeEntity[1] { entity }.ToModel();
        }

        public async Task<AlgoClientRuntimeData> GetAlgoRuntimeDataByAlgo(string clientId, string algoId)
        {
            var entities = await _table.GetDataAsync(clientId, entity => entity.AlgoId == algoId);

            return entities.ToModel();
        }

        public async Task SaveAlgoRuntimeData(AlgoClientRuntimeData data)
        {
            var enitites = data.ToEntity();

            await _table.InsertOrMergeBatchAsync(enitites);
        }

        public async Task<bool> DeleteAlgoRuntimeData(string clientId, string imageId)
        {
            var entity = await _table.DeleteAsync(clientId, imageId);
            return entity != null;
        }
    }
}
