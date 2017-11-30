using Lykke.AlgoStore.Core.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    //public class AlgoBlobRepository : IAlgoBlobBaseRepository
    //{
    //    private IAlgoBlobRepository<byte[]> _byteRepo;
    //    private IAlgoBlobRepository<string> _stringRepo;

    //    public AlgoBlobRepository(IAlgoBlobRepository<byte[]> byteRepo, IAlgoBlobRepository<string> stringRepo )
    //    {
    //        _byteRepo = byteRepo;
    //        _stringRepo = stringRepo;
    //    }

    //    public async Task<byte[]> GetBlobAsByteArrayAsync(string blobKey)
    //    {
    //        return await _byteRepo.GetBlobAsync(blobKey);
    //    }

    //    public async Task<string> GetBlobAsTextAsync(string blobKey)
    //    {
    //        return await _stringRepo.GetBlobAsync(blobKey);
    //    }

    //    public async Task SaveBlobAsByteArrayAsync(string blobKey, byte[] blobData)
    //    {
    //        await _byteRepo.SaveBlobAsync(blobKey, blobData);
    //    }

    //    public async Task SaveBlobAsStringAsync(string blobKey, string blobData)
    //    {
    //        await _stringRepo.SaveBlobAsync(blobKey, blobData);
    //    }        
    //}
}
