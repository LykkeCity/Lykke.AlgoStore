using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreClientsService
    {
        Task<List<ClientWalletData>> GetAvailableClientWalletsAsync(string clientId);
    }
}
