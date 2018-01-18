using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoClientInstanceRepository : IAlgoClientInstanceRepository
    {
        public static readonly string TableName = "AlgoClientInstanceTable";

        private readonly INoSQLTableStorage<AlgoClientInstanceEntity> _table;

        public AlgoClientInstanceRepository(INoSQLTableStorage<AlgoClientInstanceEntity> table)
        {
            _table = table;
        }

        public async Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataAsync(string clientId, string algoId)
        {
            var entities = await _table.GetDataAsync(AlgoClientInstanceMapper.GenerateKey(clientId, algoId));

            var result = entities.Select(entity => entity.ToModel()).ToList();

            return result;
        }
        public async Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(string clientId, string algoId, string instanceId)
        {
            var entitiy = await _table.GetDataAsync(AlgoClientInstanceMapper.GenerateKey(clientId, algoId), instanceId);

            return entitiy.ToModel();
        }
        public async Task<bool> ExistsAlgoInstanceDataAsync(string clientId, string algoId, string instanceId)
        {
            var entity = new AlgoClientInstanceEntity();
            entity.PartitionKey = AlgoClientInstanceMapper.GenerateKey(clientId, algoId);
            entity.RowKey = instanceId;

            return await _table.RecordExistsAsync(entity);
        }

        public async Task SaveAlgoInstanceDataAsync(AlgoClientInstanceData data)
        {
            var enitites = data.ToEntity();

            await _table.InsertOrMergeAsync(enitites);
        }
        public async Task DeleteAlgoInstanceDataAsync(AlgoClientInstanceData data)
        {
            var entities = data.ToEntity();
            await _table.DeleteAsync(entities);
        }
    }
}
