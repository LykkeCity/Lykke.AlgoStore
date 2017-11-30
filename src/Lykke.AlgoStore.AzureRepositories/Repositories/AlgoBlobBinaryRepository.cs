using System.IO;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Blob;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoBlobBinaryRepository : IAlgoBlobRepository<byte[]>
    {
        private const string BlobContainer = "algo-store-binary";
        private readonly IBlobStorage _storage;

        public AlgoBlobBinaryRepository(IReloadingManager<string> connectionStringManager)
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

        public async Task<byte[]> GetBlobAsync(string blobKey)
        {
            var stream = await _storage.GetAsync(BlobContainer, blobKey);
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
