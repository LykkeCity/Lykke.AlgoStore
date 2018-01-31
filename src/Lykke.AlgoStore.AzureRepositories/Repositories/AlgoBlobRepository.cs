using System.IO;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.AlgoStore.Core.Domain.Repositories;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoBlobRepository : IAlgoBlobRepository
    {
        public const string BlobContainer = "algo-store-binary";
        private readonly Encoding _encoding = Encoding.Unicode;

        private readonly IBlobStorage _storage;

        public AlgoBlobRepository(IBlobStorage storage)
        {
            _storage = storage;
        }

        public async Task<bool> BlobExistsAsync(string blobKey)
        {
            return await _storage.HasBlobAsync(BlobContainer, blobKey);
        }
        public async Task<byte[]> GetBlobAsync(string blobKey)
        {
            if (!await BlobExistsAsync(blobKey))
                return null;

            using (var stream = await _storage.GetAsync(BlobContainer, blobKey))
            {
                return stream.ToBytes();
            }
        }
        public async Task<string> GetBlobStringAsync(string blobKey)
        {
            if (!await BlobExistsAsync(blobKey))
                return null;

            using (var stream = await _storage.GetAsync(BlobContainer, blobKey))
            {
                return _encoding.GetString(stream.ToBytes());
            }
        }

        public async Task DeleteBlobAsync(string blobKey)
        {
            await _storage.DelBlobAsync(BlobContainer, blobKey);
        }
        public async Task SaveBlobAsync(string blobKey, Stream stream)
        {
            await _storage.SaveBlobAsync(BlobContainer, blobKey, stream);
        }
        public async Task SaveBlobAsync(string blobKey, string blobString)
        {
            using (var stream = new MemoryStream(_encoding.GetBytes(blobString)))
            {
                await SaveBlobAsync(blobKey, stream);
            }
        }
    }
}
