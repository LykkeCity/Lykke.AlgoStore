using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<AlgoClientMetaData> GetClientMetadata(string clientId);
        Task<AlgoClientMetaData> SaveClientMetadata(string clientId, AlgoMetaData data);
        Task CascadeDeleteClientMetadata(string clientId, AlgoMetaData data);
        Task<AlgoClientRuntimeData> GetRuntimeData(string clientId, string algoId);
        Task SaveAlgoAsString(string clientId, string algoId, string data);
        Task SaveAlgoAsBinary(string clientId, UploadAlgoBinaryData dataModel);
    }
}
