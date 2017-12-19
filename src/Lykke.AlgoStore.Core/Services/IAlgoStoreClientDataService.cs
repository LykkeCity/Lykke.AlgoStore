using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<AlgoClientMetaData> GetClientMetadata(string clientId);
        Task<AlgoClientMetaData> SaveClientMetadata(string clientId, AlgoMetaData data);
        Task ValidateCascadeDeleteClientMetadataRequest(string clientId, AlgoMetaData data);
        Task<AlgoClientRuntimeData> GetRuntimeData(string clientAlgoId);
        Task SaveAlgoAsString(string key, string data);
        Task SaveAlgoAsBinary(UploadAlgoBinaryData dataModel);
        bool ShouldCascadeDeleteRuntimeData(AlgoClientRuntimeData runtimeData);
        Task DeleteMetadata(string clientId, AlgoMetaData data);
        Task<bool> DeleteAlgoRuntimeData(string imageId);
    }
}
