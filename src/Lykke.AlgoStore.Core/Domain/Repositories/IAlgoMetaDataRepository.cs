using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoMetaDataRepository
    {
        Task<AlgoClientMetaData> GetAllClientMetaData(string clientId);
        Task<AlgoClientMetaData> GetClientMetaData(string id);
        Task SaveClientMetaData(AlgoClientMetaData metaData);
        Task DeleteClientMetaData(AlgoClientMetaData metaData);
    }
}
