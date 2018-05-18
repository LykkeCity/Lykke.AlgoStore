using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoBlobReadOnlyRepository
    {
        Task<bool> BlobExistsAsync(string blobKey);
        Task<byte[]> GetBlobAsync(string blobKey);
        Task<string> GetBlobStringAsync(string blobKey);
        string SourceExtension { get; }
    }
}
