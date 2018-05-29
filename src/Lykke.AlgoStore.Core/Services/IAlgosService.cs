using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgosService
    {
        Task<List<AlgoRatingMetaData>> GetAllAlgosWithRatingAsync();

        Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data, string clientId);
        Task<PublicAlgoData> RemoveFromPublicAsync(PublicAlgoData data, string clientId);

        Task SaveAlgoAsBinaryAsync(string clientId, UploadAlgoBinaryData dataModel);
        Task DeleteMetadataAsync(ManageImageData data);
        Task SaveAlgoAsStringAsync(string clientId, UploadAlgoStringData dataModel);
        Task<string> GetAlgoAsStringAsync(string clientId, string algoId);

        Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataByClientIdAsync(string clientId);

        Task<AlgoDataInformation> GetAlgoDataInformationAsync(string clientId, string algoId);
        Task<AlgoRatingData> SaveAlgoRatingAsync(AlgoRatingData data);
        Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId);
        Task<AlgoRatingData> GetAlgoRatingAsync(string algoId, string clientId);
        Task<AlgoData> CreateAlgoAsync(AlgoData data, string algoContent);
        Task<AlgoData> EditAlgoAsync(AlgoData data, string algoContent);
    }
}
