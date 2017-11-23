using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoClientRuntimeDataRepository
    {
        Task<AlgoClientRuntimeData> GetAlgoRuntimeData(string clientId, string imageId);
        Task<AlgoClientRuntimeData> GetAlgoRuntimeDataByAlgo(string clientId, string algoId);
        Task SaveAlgoRuntimeData(AlgoClientRuntimeData data);
        Task<bool> DeleteAlgoRuntimeData(string clientId, string algoId);
    }
}
