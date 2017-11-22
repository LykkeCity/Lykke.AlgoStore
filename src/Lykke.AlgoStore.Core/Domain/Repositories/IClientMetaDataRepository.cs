using Lykke.AlgoStore.Core.Domain.Entities;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IClientMetaDataRepository
    {
        Task<ClientAlgoMetaData> GetClientAlgoMetaData(string clientId);
        Task SaveClientAlgoMetaData(ClientAlgoMetaData metaData);
        Task<AlgoData> GetAlgoData(string algoId);
        Task SaveAlgoData(AlgoData metaData);
    }
}
