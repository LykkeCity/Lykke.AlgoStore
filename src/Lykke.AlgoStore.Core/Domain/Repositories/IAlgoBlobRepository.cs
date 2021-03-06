﻿using System.IO;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoBlobRepository : IAlgoBlobReadOnlyRepository
    {
        Task SaveBlobAsync(string blobKey, string blobString);
        Task SaveBlobAsync(string blobKey, Stream stream);
        Task DeleteBlobAsync(string blobKey);
    }
}
