using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Utils
{
    public class StorageConnectionManager : IStorageConnectionManager
    {
        private readonly IReloadingManager<string> _reloadingDbManager;
        private CloudStorageAccount _storageAccount;

        public StorageConnectionManager(IReloadingManager<string> reloadingDbManager)
        {
            _reloadingDbManager = reloadingDbManager;
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            string connection = await _reloadingDbManager.Reload();
            var storageAccount = CloudStorageAccount.Parse(connection);
            _storageAccount = storageAccount;
        }

        public async Task Refresh()
        {
            await Initialize();
        }

        public StorageConnectionData GetData(string key)
        {
            var result = new StorageConnectionData();
            result.StorageAccountName = _storageAccount.Credentials.AccountName;
            result.ContainerName = "algo-store-binary";
            result.AccessKey = _storageAccount.Credentials.ExportBase64EncodedKey();

            return result;
        }
    }
}
