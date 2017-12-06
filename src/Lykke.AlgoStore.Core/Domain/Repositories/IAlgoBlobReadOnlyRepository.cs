using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoBlobReadOnlyRepository
    {
        Task<bool> BlobExists(string blobKey);
        Task<byte[]> GetBlobAsync(string blobKey);
        Task<string> GetBlobStringAsync(string blobKey);
    }
}
