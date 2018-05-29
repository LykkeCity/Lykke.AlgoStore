using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoInstancesService
    {
        Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataByAlgoIdAndClientIdAsync(CSharp.AlgoTemplate.Models.Models.BaseAlgoData data);
        Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(BaseAlgoInstance data);
        Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(string clientId, string instanceId);
        Task<AlgoClientInstanceData> SaveAlgoInstanceDataAsync(AlgoClientInstanceData data, string algoClientId);
        Task<AlgoClientInstanceData> SaveAlgoBackTestInstanceDataAsync(AlgoClientInstanceData data, string algoClientId);
        Task<AlgoClientInstanceData> ValidateCascadeDeleteClientMetadataRequestAsync(ManageImageData data);
    }
}
