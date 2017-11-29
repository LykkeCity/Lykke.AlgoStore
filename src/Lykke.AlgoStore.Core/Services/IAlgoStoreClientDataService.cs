using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientDataService
    {
        Task<AlgoClientMetaData> GetClientMetadata(string clientId);
        Task<AlgoClientMetaData> SaveClientMetadata(string clientId, AlgoMetaData data);
        Task CascadeDeleteClientMetadata(string clientId, AlgoMetaData data);
        Task<List<AlgoTemplateData>> GetTemplate(string languageId);
        Task<AlgoData> GetSource(string clientAlgoId);
        Task SaveSource(AlgoData data);
        Task<AlgoClientRuntimeData> GetRuntimeData(string clientAlgoId);
        Task SaveAlgoAsString(string key, string data);
        Task SaveAlgoAsBinary(string key, IFormFile data);
    }
}
