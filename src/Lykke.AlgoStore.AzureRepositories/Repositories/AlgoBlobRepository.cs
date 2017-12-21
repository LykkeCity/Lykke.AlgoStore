using System.IO;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.Core.Domain.Repositories;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoBlobRepository : IAlgoBlobRepository
    {
        private const string BlobContainer = "algo-store-binary";
        private readonly IBlobStorage _storage;

        public AlgoBlobRepository(IBlobStorage storage)
        {
            _storage = storage;
        }

        public async Task<bool> BlobExists(string blobKey)
        {
            return await _storage.HasBlobAsync(BlobContainer, blobKey);
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
        public async Task<string> GetBlobStringAsync(string blobKey)
        {
            return await _storage.GetAsTextAsync(BlobContainer, blobKey);
        }

        public async Task DeleteBlobAsync(string blobKey)
        {
            await _storage.DelBlobAsync(BlobContainer, blobKey);
        }
        public async Task SaveBlobAsync(string blobKey, byte[] blobData)
        {
            await _storage.SaveBlobAsync(BlobContainer, blobKey, blobData);
        }
        public async Task SaveBlobAsync(string blobKey, string blobString)
        {
            await _storage.SaveBlobAsync(BlobContainer, blobKey, Encoding.UTF8.GetBytes(blobString));
        }
    }
}
