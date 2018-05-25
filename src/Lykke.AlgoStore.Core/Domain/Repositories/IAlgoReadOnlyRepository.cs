using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoReadOnlyRepository
    {
        Task<IEnumerable<IAlgo>> GetAllAlgosAsync();
        Task<IEnumerable<IAlgo>> GetAllClientAlgosAsync(string clientId);
        Task<IAlgo> GetAlgoAsync(string clientId, string algoId);
        Task<bool> ExistsAlgoMetaDataAsync(string clientId, string algoId);
        Task<AlgoDataInformation> GetAlgoDataInformationAsync(string clientId, string algoId);
    }
}
