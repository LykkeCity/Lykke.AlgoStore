using AzureStorage;
using AzureStorage.Blob;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoBlobBinaryRepository : IAlgoBlobRepository<byte[]>  //mock with AzureStorage.Blob.AzureBlobInMemory
    {
        private const string BlobContainer = "algo-store-binary";
        private readonly IBlobStorage _storage;

        public AlgoBlobBinaryRepository(IReloadingManager<string> connectionStringManager)
        {
            _storage = AzureBlobStorage.Create(connectionStringManager);
        }

        public async Task<byte[]> GetBlobAsync(string blobKey)
        {
            var stream  = await _storage.GetAsync(BlobContainer, blobKey);
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public async Task SaveBlobAsync(string blobKey, byte[] blobData)
        {
           await _storage.SaveBlobAsync(BlobContainer, blobKey, blobData);
        }      
    }
}
