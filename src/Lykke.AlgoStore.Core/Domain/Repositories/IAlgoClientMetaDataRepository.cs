using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoClientMetaDataRepository
    {
        Task<AlgoClientMetaData> GetAllClientMetaData(string clientId);
        Task<AlgoClientMetaData> GetClientMetaData(string clientId, string id);
        Task SaveClientMetaData(AlgoClientMetaData metaData);
        Task DeleteClientMetaData(AlgoClientMetaData metaData);
    }
}
