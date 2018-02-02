using System;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class RandomAlgoRatingsRepository : IAlgoRatingsRepository
    {
        private static readonly Random Rnd = new Random();

        public AlgoRatingData GetAlgoRating(string clientId, string algoId)
        {
            var result = new AlgoRatingData
            {
                Rating = Math.Round(Rnd.NextDouble() * (6 - 1) + 1, 2),
                UsersCount = Rnd.Next(0, 201)
            };

            return result;
        }
    }
}
