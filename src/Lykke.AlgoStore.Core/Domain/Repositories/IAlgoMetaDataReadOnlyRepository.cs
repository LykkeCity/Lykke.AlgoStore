using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoMetaDataReadOnlyRepository
    {
        Task<AlgoClientMetaData> GetAllClientAlgoMetaData(string clientId);
        Task<AlgoClientMetaData> GetAlgoMetaData(string clientId, string algoId);
        Task<bool> ExistsAlgoMetaData(string clientId, string algoId);
    }
}
