using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoBlobRepository<T>
    {
        Task<T> GetBlobAsync(string blobKey);
        Task SaveBlobAsync(string blobKey, T blobData);
        Task DeleteBlobAsync(string blobKey);
        Task<bool> BlobExists(string blobKey);
    }
}
