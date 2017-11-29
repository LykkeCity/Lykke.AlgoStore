using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoBlobBaseRepository
    {
        Task<string> GetBlobAsTextAsync(string blobKey);
        Task SaveBlobAsStringAsync(string blobKey, string blobData);
        Task<byte[]> GetBlobAsByteArrayAsync(string blobKey);
        Task SaveBlobAsByteArrayAsync(string blobKey, byte[] blobData);
    }
}
