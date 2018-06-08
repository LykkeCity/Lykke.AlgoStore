﻿using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgosService
    {
        Task<List<AlgoRatingMetaData>> GetAllAlgosWithRatingAsync();
        Task<List<AlgoData>> GetAllUserAlgosAsync(string clientId);

        Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data, string clientId);
        Task<PublicAlgoData> RemoveFromPublicAsync(PublicAlgoData data, string clientId);

        Task SaveAlgoAsBinaryAsync(string clientId, UploadAlgoBinaryData dataModel);
        Task DeleteAsync(ManageImageData data);
        Task DeleteAlgoAsync(string algoClientId, string algoId, bool forceDelete, string clientId);
        Task SaveAlgoAsStringAsync(string clientId, UploadAlgoStringData dataModel);
        Task<string> GetAlgoAsStringAsync(string clientId, string algoId);

        Task<AlgoDataInformation> GetAlgoDataInformationAsync(string clientId, string algoClientId, string algoId);
        Task<AlgoRatingData> SaveAlgoRatingAsync(AlgoRatingData data);
        Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId);
        Task<AlgoRatingData> GetAlgoRatingAsync(string algoId, string clientId);
        Task<AlgoData> CreateAlgoAsync(AlgoData data, string algoContent);
        Task<AlgoData> EditAlgoAsync(AlgoData data, string algoContent);
    }
}
