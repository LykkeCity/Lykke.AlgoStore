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
    //Reserved for future use
    public class AlgoTemplateDataRepository
    {
        private const string PartitionKey = "AlgoTemplateData";
        private const string TableName = "AlgoTemplateDataTable";

        private readonly INoSQLTableStorage<AlgoTemplateDataEntity> _table;

        public AlgoTemplateDataRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _table = AzureTableStorage<AlgoTemplateDataEntity>.Create(connectionStringManager, TableName, log);
        }

        public async Task<List<AlgoTemplateData>> GetTemplatesByLanguage(string languageId)
        {
            var entities = await _table.GetDataAsync(PartitionKey, entity => entity.LanguageId == languageId && entity.IsActive == true);

            return entities.ToModel();
        }
    }
}
