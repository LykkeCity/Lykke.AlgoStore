using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IPublicAlgosRepository
    {
        Task<List<PublicAlgoData>> GetAllPublicAlgosAsync();
        Task<bool> ExistsPublicAlgoAsync(string clientId, string algoId);
        Task SavePublicAlgoAsync(PublicAlgoData data);
        Task DeletePublicAlgoAsync(PublicAlgoData data);
    }
}
