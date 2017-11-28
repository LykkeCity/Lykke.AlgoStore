using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoMetaDataRepository
    {
        Task<AlgoClientMetaData> GetAllClientAlgoMetaData(string clientId);
        Task<AlgoClientMetaData> GetAlgoMetaData(string id);
        Task<AlgoClientMetaData> SaveAlgoMetaData(AlgoClientMetaData metaData);
        Task DeleteAlgoMetaData(AlgoClientMetaData metaData);
    }
}
