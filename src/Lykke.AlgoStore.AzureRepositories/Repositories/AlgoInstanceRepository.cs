using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Enumerators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoInstanceRepository : IAlgoInstanceRepository
    {
        private readonly INoSQLTableStorage<AlgoInstanceEntity> _table;

        public static readonly string TableName = "AlgoClientInstanceTable";

        public AlgoInstanceRepository(INoSQLTableStorage<AlgoInstanceEntity> table)
        {
            _table = table;
        }

        public async Task<List<string>> GetInstanceWalletIdsByStatusAsync(string clientId, params AlgoInstanceStatus[] statuses)
        {
            var entities = await _table.GetDataAsync(KeyGenerator.GenerateClientIdPartitionKey(clientId));

            var filteredByStatus = entities.Where(e => statuses.Contains(e.AlgoInstanceStatus)).Select(f => f.WalletId).ToList();

            return filteredByStatus;
        }
    }
}
