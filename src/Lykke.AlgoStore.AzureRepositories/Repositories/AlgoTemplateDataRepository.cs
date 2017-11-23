using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoTemplateDataRepository
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
    }
}
