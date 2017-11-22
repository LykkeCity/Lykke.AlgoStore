using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class ClientMetaDataRepository : IClientMetaDataRepository
    {
        private const string ClientAlgoMetaDataPartitionKey = "ClientAlgoMetaData";

        private readonly INoSQLTableStorage<ClientAlgoMetaDataEntity> _table;
        private readonly IReloadingManager<string> _connectionStringManager;
        private readonly ILog _log;

        public ClientMetaDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _log = log;
            _connectionStringManager = connectionStringManager;
            _table = AzureTableStorage<ClientAlgoMetaDataEntity>.Create(connectionStringManager, ClientAlgoMetaDataPartitionKey, _log);
        }

        public async Task<ClientAlgoMetaData> GetClientAlgoMetaData(string clientId)
        {
            var entities = await _table.GetDataAsync(clientId);

            return entities.ToModel();
        }
        public async Task SaveClientAlgoMetaData(ClientAlgoMetaData metaData)
        {
            var enitites = metaData.ToEntity();

            await _table.InsertOrMergeBatchAsync(enitites);
        }


        public async Task<AlgoData> GetAlgoData(string algoId)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAlgoData(AlgoData metaData)
        {
            throw new NotImplementedException();
        }
    }
}
