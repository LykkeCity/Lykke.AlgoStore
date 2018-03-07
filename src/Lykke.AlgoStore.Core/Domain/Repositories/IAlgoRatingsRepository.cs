using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRatingsRepository
    {
        Task<List<AlgoRatingData>> GetAlgoRating(string algoId);
        Task<AlgoRatingData> GetAlgoRatingForClient(string clientId, string algoId);
        Task SaveAlgoRating(AlgoRatingData data);
    }
}
