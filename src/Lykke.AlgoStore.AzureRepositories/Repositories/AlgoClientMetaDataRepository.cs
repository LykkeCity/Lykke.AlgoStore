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
    public class AlgoClientMetaDataRepository : IAlgoClientMetaDataRepository
    {
        private const string ClientAlgoMetaDataTableName = "ClientAlgoMetaData";

        private readonly INoSQLTableStorage<AlgoClientMetaDataEntity> _table;
        private readonly IReloadingManager<string> _connectionStringManager;
        private readonly ILog _log;

        public AlgoClientMetaDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _log = log;
            _connectionStringManager = connectionStringManager;
            _table = AzureTableStorage<AlgoClientMetaDataEntity>.Create(connectionStringManager, ClientAlgoMetaDataTableName, _log);
        }

        public async Task<AlgoClientMetaData> GetClientMetaData(string clientId)
        {
            var entities = await _table.GetDataAsync(clientId);

            return entities.ToModel();
        }
        public async Task SaveClientMetaData(AlgoClientMetaData metaData)
        {
            var enitites = metaData.ToEntity();

            await _table.InsertOrMergeBatchAsync(enitites);
        }
        public async Task<bool> DeleteClientMetaData(string clientId, string clientMetadataId)
        {
            var entity = await _table.DeleteAsync(clientId, clientMetadataId);
            return entity != null;
        }
    }
}
