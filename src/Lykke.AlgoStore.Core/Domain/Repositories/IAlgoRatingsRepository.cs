using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRatingsRepository
    {
        Task<List<AlgoRatingData>> GetAlgoRatingsAsync(string algoId);
        Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId);
        Task SaveAlgoRatingAsync(AlgoRatingData data);
        Task DeleteRatingsAsync(string algoId);
    }
}
