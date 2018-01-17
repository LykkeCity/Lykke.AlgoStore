using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoClientInstanceReadOnlyRepository
    {
        Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceData(string clientId, string algoId);
        Task<AlgoClientInstanceData> GetAlgoInstanceData(string clientId, string algoId, string instanceId);
        Task<bool> ExistsAlgoInstanceData(string clientId, string algoId, string instanceId);
    }
}
