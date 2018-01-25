using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public class RandomAlgoRatingsRepository : IAlgoRatingsRepository
    {
        Random rnd = new Random();
        public double GetAlgoRating()
        {
            return Math.Round(rnd.NextDouble() * (6 - 1) + 1, 2);
        }
    }
}
