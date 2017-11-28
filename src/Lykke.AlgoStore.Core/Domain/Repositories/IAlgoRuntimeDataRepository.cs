using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRuntimeDataRepository
    {
        Task<AlgoClientRuntimeData> GetAlgoRuntimeData(string imageId);
        Task<AlgoClientRuntimeData> GetAlgoRuntimeDataByAlgo(string algoId);
        Task SaveAlgoRuntimeData(AlgoClientRuntimeData data);
        Task<bool> DeleteAlgoRuntimeData(string algoId);
    }
}
