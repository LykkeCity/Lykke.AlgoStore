using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRepository : IAlgoReadOnlyRepository
    {
        Task SaveAlgoAsync(IAlgo metaData);

        Task DeleteAlgoAsync(string clientId, string algoId);
    }
}
