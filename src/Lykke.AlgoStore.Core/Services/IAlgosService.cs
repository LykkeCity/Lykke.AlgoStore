using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgosService
    {
        Task<List<AlgoRatingMetaData>> GetAllAlgosWithRatingAsync();
        Task<List<AlgoData>> GetAllUserAlgosAsync(string clientId);

        Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data);
        Task<PublicAlgoData> RemoveFromPublicAsync(PublicAlgoData data);

        Task SaveAlgoAsBinaryAsync(string clientId, UploadAlgoBinaryData dataModel);
        Task DeleteAsync(ManageImageData data);
        Task DeleteAlgoAsync(string algoClientId, string algoId, bool forceDelete, string clientId);
        Task SaveAlgoAsStringAsync(string clientId, UploadAlgoStringData dataModel);
        Task<string> GetAlgoAsStringAsync(string clientId, string algoId);

        Task<AlgoDataInformation> GetAlgoDataInformationAsync(string clientId, string algoId);
        Task<AlgoRatingData> SaveAlgoRatingAsync(AlgoRatingData data);
        Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId);
        Task<AlgoRatingData> GetAlgoRatingAsync(string algoId, string clientId);
        Task<AlgoData> CreateAlgoAsync(AlgoData data, string algoContent);
        Task<AlgoData> EditAlgoAsync(AlgoData data, string algoContent);
        Task<List<EnumValue>> GetAssetsForAssetPairAsync(string assetPairId, string clientId);
        Task<bool> GetIsLoggedUserCreatorOfAlgo(string algoId, string clientId);
    }
}
