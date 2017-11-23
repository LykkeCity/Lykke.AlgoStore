using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoDataRepository
    {
        Task<AlgoData> GetAlgoData(string clientId, string algoId);
        Task SaveAlgoData(AlgoData metaData);
        Task<bool> DeleteAlgoData(string clientId, string algoId);
    }
}
