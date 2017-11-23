using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoClientMetaDataRepository
    {
        Task<AlgoClientMetaData> GetClientMetaData(string clientId);
        Task SaveClientMetaData(AlgoClientMetaData metaData);
        Task<bool> DeleteClientMetaData(string clientId, string clientMetadataId);
    }
}
