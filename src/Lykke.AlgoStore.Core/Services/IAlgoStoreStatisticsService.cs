using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreStatisticsService
    {
        Task<StatisticsSummary> GetStatisticsSummaryAsync(string clientId, string instanceId);
        Task<StatisticsSummary> UpdateStatisticsSummaryAsync(string clientId, string instanceId);
    }
}
