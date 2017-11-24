using System.Collections.Generic;
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
    public class AlgoTemplateDataRepository : IAlgoTemplateDataRepository
    {
        private const string PartitionKey = "AlgoTemplateData";
        private const string TableName = "AlgoTemplateDataTable";

        private readonly INoSQLTableStorage<AlgoTemplateDataEntity> _table;
        private readonly IReloadingManager<string> _connectionStringManager;
        private readonly ILog _log;

        public AlgoTemplateDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _log = log;
            _connectionStringManager = connectionStringManager;
            _table = AzureTableStorage<AlgoTemplateDataEntity>.Create(connectionStringManager, TableName, _log);
        }

        public async Task<List<AlgoTemplateData>> GetTemplatesByLanguage(string languageId)
        {
            var entities = await _table.GetDataAsync(PartitionKey, entity => entity.LanguageId == languageId && entity.IsActive == true);

            return entities.ToModel();
        }
    }
}
