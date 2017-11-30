using AzureStorage;
using AzureStorage.Blob;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoBlobStringRepository : IAlgoBlobRepository<string> //mock with AzureStorage.Blob.AzureBlobInMemory
    {
        private const string BlobContainer = "algo-store-string";
        private readonly IBlobStorage _storage;

        public AlgoBlobStringRepository(IReloadingManager<string> connectionStringManager)
        {
            _storage = AzureBlobStorage.Create(connectionStringManager);
        }

        public async Task<bool> BlobExists(string blobKey)
        {
            return await _storage.HasBlobAsync(BlobContainer, blobKey);
        }

        public async Task DeleteBlobAsync(string blobKey)
        {
            await _storage.DelBlobAsync(BlobContainer, blobKey);
        }

        public async Task<string> GetBlobAsync(string blobKey)
        {
            return await _storage.GetAsTextAsync(BlobContainer, blobKey);
        }

        public async Task SaveBlobAsync(string blobKey, string blobData)
        {
            await _storage.SaveBlobAsync(BlobContainer, blobKey, Encoding.UTF8.GetBytes(blobData));
        }
    }
}
