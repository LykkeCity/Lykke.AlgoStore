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
    public class AlgoDataRepository : IAlgoDataRepository
    {
        private const string AlgoDataTableName = "AlgoData";

        private readonly INoSQLTableStorage<AlgoDataEntity> _table;
        private readonly IReloadingManager<string> _connectionStringManager;
        private readonly ILog _log;

        public AlgoDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _log = log;
            _connectionStringManager = connectionStringManager;
            _table = AzureTableStorage<AlgoDataEntity>.Create(connectionStringManager, AlgoDataTableName, _log);
        }

        public async Task<AlgoData> GetAlgoData(string clientId, string algoId)
        {
            var entity = await _table.GetDataAsync(clientId, algoId);

            return entity.ToModel();
        }
        public async Task SaveAlgoData(AlgoData metaData)
        {
            var enitity = metaData.ToEntity();

            await _table.InsertOrMergeAsync(enitity);
        }
        public async Task<bool> DeleteAlgoData(string clientId, string algoId)
        {
            var entity = await _table.DeleteAsync(clientId, algoId);
            return entity != null;
        }
    }
}
