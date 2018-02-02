using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<List<AlgoRatingMetaData>> GetAllAlgosWithRatingAsync();
        Task<AlgoClientMetaData> GetClientMetadataAsync(string clientId);
        Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data);
        Task<AlgoClientMetaData> SaveClientMetadataAsync(string clientId, string clientName, AlgoMetaData data);
        Task<AlgoClientRuntimeData> ValidateCascadeDeleteClientMetadataRequestAsync(string clientId, AlgoMetaData data);
        Task SaveAlgoAsBinaryAsync(string clientId, UploadAlgoBinaryData dataModel);
        Task DeleteMetadataAsync(string clientId, AlgoMetaData data);
        Task SaveAlgoAsStringAsync(string clientId, UploadAlgoStringData dataModel);
        Task<string> GetAlgoAsStringAsync(string clientId, string algoId);
        Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataAsync(BaseAlgoData data);
        Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(BaseAlgoInstance data);
        Task<AlgoClientInstanceData> SaveAlgoInstanceDataAsync(AlgoClientInstanceData data);
    }
}
