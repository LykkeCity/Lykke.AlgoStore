using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoMetaDataReadOnlyRepository
    {
        Task<AlgoClientMetaData> GetAllAlgos();
        Task<AlgoClientMetaData> GetAllClientAlgoMetaDataAsync(string clientId);
        Task<AlgoClientMetaData> GetAlgoMetaDataAsync(string clientId, string algoId);
        Task<bool> ExistsAlgoMetaDataAsync(string clientId, string algoId);
        Task<AlgoClientMetaDataInformation> GetAlgoMetaDataInformationAsync(string clientId, string algoId);
    }
}
