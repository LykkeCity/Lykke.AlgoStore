using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<BaseDataServiceResult<AlgoClientMetaData>> GetClientMetadata(string clientId);
        Task<BaseDataServiceResult<AlgoClientMetaData>> SaveClientMetadata(string clientId, AlgoMetaData data);
        Task<BaseServiceResult> DeleteClientMetadata(string clientId, AlgoMetaData data);
    }
}
