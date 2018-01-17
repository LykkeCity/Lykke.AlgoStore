using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRuntimeDataRepository : IAlgoRuntimeDataReadOnlyRepository
    {
        Task SaveAlgoRuntimeDataAsync(AlgoClientRuntimeData data);
        Task<bool> DeleteAlgoRuntimeDataAsync(string clientId, string algoId);
    }
}
