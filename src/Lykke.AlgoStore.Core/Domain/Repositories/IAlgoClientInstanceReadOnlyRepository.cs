using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoClientInstanceReadOnlyRepository
    {
        Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataAsync(string clientId, string algoId);
        Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(string clientId, string algoId, string instanceId);
        Task<bool> ExistsAlgoInstanceDataAsync(string clientId, string algoId, string instanceId);
        Task<bool> HasInstanceData(string clientId, string algoId);
    }
}
