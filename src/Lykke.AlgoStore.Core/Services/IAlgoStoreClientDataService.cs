using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<AlgoClientMetaData> GetClientMetadata(string clientId);
        Task<AlgoClientMetaData> SaveClientMetadata(string clientId, AlgoMetaData data);
        Task CascadeDeleteClientMetadata(string clientId, AlgoMetaData data);
        Task<AlgoClientRuntimeData> GetRuntimeData(string clientAlgoId);
        Task SaveAlgoAsString(string key, string data);
        Task SaveAlgoAsBinary(UploadAlgoBinaryData dataModel);
        Task<string> GetTestLog(string clientAlgoId);
    }
}
