using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoRatingsRepository
    {
        AlgoRatingData GetAlgoRating(string clientId, string algoId);
    }
}
