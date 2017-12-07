using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRuntimeDataRepository : IAlgoRuntimeDataReadOnlyRepository
    {
        Task SaveAlgoRuntimeData(AlgoClientRuntimeData data);
        Task<bool> DeleteAlgoRuntimeData(string algoId);
    }
}
