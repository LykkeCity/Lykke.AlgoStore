using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRatingsRepository
    {
        Task<List<AlgoRatingData>> GetAlgoRatingAsync(string algoId);
        Task<AlgoRatingData> GetAlgoRatingForClientAsync(string clientId, string algoId);
        Task SaveAlgoRatingAsync(AlgoRatingData data);
    }
}
