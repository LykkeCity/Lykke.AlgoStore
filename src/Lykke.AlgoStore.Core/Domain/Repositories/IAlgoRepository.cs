using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRepository : IAlgoReadOnlyRepository
    {
        Task SaveAlgoAsync(IAlgo metaData);
        Task SaveAlgoWithNewPKAsync(IAlgo algo, string oldPK);
        Task DeleteAlgoAsync(string clientId, string algoId);
    }
}
