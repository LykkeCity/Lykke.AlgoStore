using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<List<AlgoRatingMetaData>> GetAllAlgosWithRatingAsync();
        Task<AlgoClientMetaData> GetClientMetadataAsync(string clientId);
        Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data);
        Task<AlgoClientMetaData> SaveClientMetadataAsync(string clientId, string clientName, AlgoMetaData data);
        Task<AlgoClientInstanceData> ValidateCascadeDeleteClientMetadataRequestAsync(ManageImageData data);
        Task SaveAlgoAsBinaryAsync(string clientId, UploadAlgoBinaryData dataModel);
        Task DeleteMetadataAsync(ManageImageData data);
        Task SaveAlgoAsStringAsync(string clientId, UploadAlgoStringData dataModel);
        Task<string> GetAlgoAsStringAsync(string clientId, string algoId);
        Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataAsync(CSharp.AlgoTemplate.Models.Models.BaseAlgoData data);
        Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(CSharp.AlgoTemplate.Models.Models.BaseAlgoInstance data);
        Task<AlgoClientInstanceData> SaveAlgoInstanceDataAsync(AlgoClientInstanceData data);
        Task<AlgoClientMetaDataInformation> GetAlgoMetaDataInformationAsync(string clientId, string algoId);
        Task<AlgoRatingData> SaveAlgoRating(AlgoRatingData data);
        Task<AlgoRatingData> GetAlgoRatingForClient(string algoId, string clientId);
        Task<AlgoRatingData> GetAlgoRating(string algoId, string clientId);
    }
}
