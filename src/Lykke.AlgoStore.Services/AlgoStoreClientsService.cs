using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Enumerators;
using Lykke.AlgoStore.Core.Services;
using Lykke.Service.ClientAccount.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreClientsService : IAlgoStoreClientsService
    {
        private readonly IClientAccountClient _clientAccountService;
        private readonly IAlgoInstanceRepository _clientInstanceRepository;

        public AlgoStoreClientsService(IClientAccountClient clientAccountService,
                                       IAlgoInstanceRepository clientInstanceRepository)
        {
            _clientAccountService = clientAccountService;
            _clientInstanceRepository = clientInstanceRepository;
        }

        public async Task<List<ClientWalletData>> GetAvailableClientWalletsAsync(string clientId)
        {
            var allWallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);

            var startedOrDeploying = await _clientInstanceRepository.GetInstanceWalletIdsByStatusAsync(clientId, new AlgoInstanceStatus[] {AlgoInstanceStatus.Started, AlgoInstanceStatus.Deploying});

            return allWallets.Where(w => !startedOrDeploying.Contains(w.Id)).Select(w => ClientWalletData.CreateFromDto(w)).ToList();
        }
    }
}
