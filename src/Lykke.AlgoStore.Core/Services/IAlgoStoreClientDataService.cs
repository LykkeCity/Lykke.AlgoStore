using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<List<AlgoRatingMetaData>> GetAllAlgosWithRatingAsync();

        Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data, string clientId);
        Task<PublicAlgoData> RemoveFromPublicAsync(PublicAlgoData data, string clientId);

        Task<AlgoClientInstanceData> ValidateCascadeDeleteClientMetadataRequestAsync(ManageImageData data);
        Task SaveAlgoAsBinaryAsync(string clientId, UploadAlgoBinaryData dataModel);
        Task DeleteMetadataAsync(ManageImageData data);
        Task SaveAlgoAsStringAsync(string clientId, UploadAlgoStringData dataModel);
        Task<string> GetAlgoAsStringAsync(string clientId, string algoId);
        Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataByAlgoIdAndClientIdAsync(CSharp.AlgoTemplate.Models.Models.BaseAlgoData data);

        Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataByClientIdAsync(string clientId);

        Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(CSharp.AlgoTemplate.Models.Models.BaseAlgoInstance data);
        Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(string clientId, string instanceId);
        Task<AlgoClientInstanceData> SaveAlgoInstanceDataAsync(AlgoClientInstanceData data, string algoClientId);
        Task<AlgoClientInstanceData> SaveAlgoBackTestInstanceDataAsync(AlgoClientInstanceData data, string algoClientId);
        Task<AlgoDataInformation> GetAlgoDataInformationAsync(string clientId, string algoId);
        Task<AlgoRatingData> SaveAlgoRatingAsync(AlgoRatingData data);
        Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId);
        Task<AlgoRatingData> GetAlgoRatingAsync(string algoId, string clientId);
        Task<AlgoData> CreateAlgoAsync(string clientId, string clientName, AlgoData data, string algoContent);
    }
}
