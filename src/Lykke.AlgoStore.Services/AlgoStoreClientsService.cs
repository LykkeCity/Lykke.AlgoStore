using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.Service.ClientAccount.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreClientsService : IAlgoStoreClientsService
    {
        private readonly IClientAccountClient _clientAccountService;
        private readonly IAlgoClientInstanceRepository _clientInstanceRepository;

        public AlgoStoreClientsService(IClientAccountClient clientAccountService,
                                       IAlgoClientInstanceRepository clientInstanceRepository)
        {
            _clientAccountService = clientAccountService;
            _clientInstanceRepository = clientInstanceRepository;
        }

        public async Task<List<ClientWalletData>> GetAvailableClientWalletsAsync(string clientId)
        {
            var allClientWallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);

            var result = new List<ClientWalletData>();

            foreach (var wallet in allClientWallets)
            {
                var startedOrDeployingInstances = await _clientInstanceRepository.GetAllByWalletIdAndInstanceStatusIsNotStoppedAsync(wallet.Id);
                if (!startedOrDeployingInstances.Any())
                {
                    result.Add(ClientWalletData.CreateFromDto(wallet));
                }
            }

            return result;
        }
    }
}
