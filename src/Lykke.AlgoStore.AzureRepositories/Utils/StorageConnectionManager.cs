using System;
using System.Globalization;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage;

namespace Lykke.AlgoStore.AzureRepositories.Utils
{
    public class StorageConnectionManager : IStorageConnectionManager
    {
        private readonly IReloadingManager<string> _reloadingDbManager;
        private CloudStorageAccount _storageAccount;

        public StorageConnectionManager(IReloadingManager<string> reloadingDbManager)
        {
            _reloadingDbManager = reloadingDbManager;
            Initialize();
        }

        private void Initialize()
        {
            var storageAccount = CloudStorageAccount.Parse(_reloadingDbManager.CurrentValue);
            _storageAccount = storageAccount;
        }

        public async Task Refresh()
        {
            await _reloadingDbManager.Reload();
            Initialize();
        }

        public string GetDateHeader()
        {
            const string dateHeaderFormat = "x-ms-date:{0}";

            return string.Format(dateHeaderFormat, DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture));
        }

        public string GetServerVersionHeader()
        {
            const string serverVersionHeaderFormat = "x-ms-version:{0}";

            return string.Format(serverVersionHeaderFormat, "2017-04-17");
        }

        public string GetAuthenticationHeader(string tableName, string partitionKey, string rowKey)
        {
            return String.Empty;
        }
    }
}
