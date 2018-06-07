using Lykke.AlgoStore.Core.Enumerators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoInstanceRepository
    {
        Task<List<string>> GetInstanceWalletIdsByStatusAsync(string clientId, params AlgoInstanceStatus[] statuses);
    }
}
