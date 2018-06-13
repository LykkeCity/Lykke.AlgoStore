using Lykke.AlgoStore.Service.AlgoTrades.Client.AutorestClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreTradesService
    {
        Task<IEnumerable<AlgoInstanceTradeResponseModel>> GetAllTradesForAlgoInstanceAsync(string clientId, string instanceId);
    }
}
