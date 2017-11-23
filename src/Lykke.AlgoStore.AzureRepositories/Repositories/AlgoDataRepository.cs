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
    public class AlgoDataRepository : IAlgoDataRepository
    {
        private const string PartitionKey = "AlgoData";
        private const string TableName = "AlgoDataTable";

        private readonly INoSQLTableStorage<AlgoDataEntity> _table;
        private readonly IReloadingManager<string> _connectionStringManager;
        private readonly ILog _log;

        public AlgoDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _log = log;
            _connectionStringManager = connectionStringManager;
            _table = AzureTableStorage<AlgoDataEntity>.Create(connectionStringManager, TableName, _log);
        }

        public async Task<AlgoData> GetAlgoData(string algoId)
        {
            var entity = await _table.GetDataAsync(PartitionKey, algoId);

            return entity.ToModel();
        }
        public async Task SaveAlgoData(AlgoData metaData)
        {
            var enitity = metaData.ToEntity(PartitionKey);

            await _table.InsertOrMergeAsync(enitity);
        }
        public async Task<bool> DeleteAlgoData(string algoId)
        {
            var entity = await _table.DeleteAsync(PartitionKey, algoId);
            return entity != null;
        }
    }
}
