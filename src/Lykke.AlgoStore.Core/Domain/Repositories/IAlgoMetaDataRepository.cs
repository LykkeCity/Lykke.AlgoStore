using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoMetaDataRepository
    {
        Task<AlgoClientMetaData> GetAllClientAlgoMetaData(string clientId);
        Task<AlgoClientMetaData> GetAlgoMetaData(string id);
        Task SaveAlgoMetaData(AlgoClientMetaData metaData);
        Task DeleteAlgoMetaData(AlgoClientMetaData metaData);
        Task<bool> ExistsAlgoMetaData(string id);
    }
}
