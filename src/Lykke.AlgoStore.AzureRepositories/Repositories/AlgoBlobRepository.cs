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
        internal const string BlobContainer = "algo-store-binary";
        private readonly Encoding _encoding = Encoding.UTF8;
        private static readonly string Extension = ".txt";

        private readonly IBlobStorage _storage;

        public AlgoBlobRepository(IBlobStorage storage)
        {
            _storage = storage;
        }

        public async Task<bool> BlobExistsAsync(string blobKey)
        {
            if (!blobKey.EndsWith(Extension))
                blobKey = blobKey + Extension;
            return await _storage.HasBlobAsync(BlobContainer, blobKey);
        }
        public async Task<byte[]> GetBlobAsync(string blobKey)
        {
            if (!await BlobExistsAsync(blobKey))
                return null;

            if (!blobKey.EndsWith(Extension))
                blobKey = blobKey + Extension;
            using (var stream = await _storage.GetAsync(BlobContainer, blobKey))
            {
                return stream.ToBytes();
            }
        }
        public async Task<string> GetBlobStringAsync(string blobKey)
        {
            if (!await BlobExistsAsync(blobKey))
                return null;

            if (!blobKey.EndsWith(Extension))
                blobKey = blobKey + Extension;
            using (var stream = await _storage.GetAsync(BlobContainer, blobKey))
            {
                return _encoding.GetString(stream.ToBytes());
            }
        }

        public async Task DeleteBlobAsync(string blobKey)
        {
            if (!blobKey.EndsWith(Extension))
                blobKey = blobKey + Extension;
            await _storage.DelBlobAsync(BlobContainer, blobKey);
        }
        public async Task SaveBlobAsync(string blobKey, Stream stream)
        {
            if (!blobKey.EndsWith(Extension))
                blobKey = blobKey + Extension;
            await _storage.SaveBlobAsync(BlobContainer, blobKey, stream);
        }
        public async Task SaveBlobAsync(string blobKey, string blobString)
        {
            if (!blobKey.EndsWith(Extension))
                blobKey = blobKey + Extension;
            using (var stream = new MemoryStream(_encoding.GetBytes(blobString)))
            {
                await SaveBlobAsync(blobKey, stream);
            }
        }

        public string SourceExtension => Extension;
    }
}
