﻿using System.Threading.Tasks;
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
    public class AlgoMetaDataRepository : IAlgoMetaDataRepository
    {
        private const string PartitionKey = "AlgoMetaData";
        private const string TableName = "AlgoMetaDataTable";

        private readonly INoSQLTableStorage<AlgoMetaDataEntity> _table;
        private readonly IReloadingManager<string> _connectionStringManager;
        private readonly ILog _log;

        public AlgoMetaDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _log = log;
            _connectionStringManager = connectionStringManager;
            _table = AzureTableStorage<AlgoMetaDataEntity>.Create(connectionStringManager, TableName, _log);
        }

        public async Task<AlgoClientMetaData> GetAllClientMetaData(string clientId)
        {
            var entities = await _table.GetDataAsync(PartitionKey, data => data.ClientId == clientId);

            return entities.ToModel();
        }
        public async Task<AlgoClientMetaData> GetClientMetaData(string id)
        {
            var entitiy = await _table.GetDataAsync(PartitionKey, id);

            return new AlgoMetaDataEntity[1] { entitiy }.ToModel();
        }
        public async Task SaveClientMetaData(AlgoClientMetaData metaData)
        {
            var enitites = metaData.ToEntity(PartitionKey);

            await _table.InsertOrMergeBatchAsync(enitites); //return the saved data to save the call to read it later to verify success

        }
        public async Task DeleteClientMetaData(AlgoClientMetaData metaData)
        {
            var entities = metaData.ToEntity(PartitionKey);
            await _table.DeleteAsync(entities);
        }
    }
}
