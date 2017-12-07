using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoBlobRepository : IAlgoBlobReadOnlyRepository
    {
        Task SaveBlobAsync(string blobKey, string blobString);
        Task SaveBlobAsync(string blobKey, byte[] blobData);
        Task DeleteBlobAsync(string blobKey);
    }
}
