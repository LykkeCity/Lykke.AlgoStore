using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<AlgoClientMetaData> GetClientMetadata(string clientId);
        Task<AlgoClientMetaData> SaveClientMetadata(string clientId, AlgoMetaData data);
        Task<AlgoClientRuntimeData> ValidateCascadeDeleteClientMetadataRequest(string clientId, AlgoMetaData data);
        Task SaveAlgoAsBinary(string clientId, UploadAlgoBinaryData dataModel);
        Task DeleteMetadata(string clientId, AlgoMetaData data);
        Task SaveAlgoAsString(string clientId, UploadAlgoStringData dataModel);
        Task<string> GetAlgoAsString(string clientId, string algoId);
    }
}
