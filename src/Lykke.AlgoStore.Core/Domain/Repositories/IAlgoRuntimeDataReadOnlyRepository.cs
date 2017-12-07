using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRuntimeDataReadOnlyRepository
    {
        Task<AlgoClientRuntimeData> GetAlgoRuntimeData(string imageId);
        Task<AlgoClientRuntimeData> GetAlgoRuntimeDataByAlgo(string algoId);
    }
}
