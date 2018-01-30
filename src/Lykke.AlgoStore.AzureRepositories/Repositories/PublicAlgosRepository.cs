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
    public class PublicAlgosRepository : IPublicAlgosRepository
    {
        public static readonly string TableName = "PublicAlgosTable";

        private readonly INoSQLTableStorage<PublicAlgoEntity> _table;

        public PublicAlgosRepository(INoSQLTableStorage<PublicAlgoEntity> table)
        {
            _table = table;
        }

        public async Task<List<PublicAlgoData>> GetAllPublicAlgosAsync()
        {
            var entities = await _table.GetDataAsync(PublicAlgoMapper.PartitionKey);

            var result = entities.Select(entity => entity.ToModel()).ToList();

            return result;
        }
        public async Task<bool> ExistsPublicAlgoAsync(string clientId, string algoId)
        {
            var entity = new PublicAlgoEntity();
            entity.PartitionKey = PublicAlgoMapper.PartitionKey;
            entity.RowKey = KeyGenerator.GenerateKey(clientId, algoId);

            return await _table.RecordExistsAsync(entity);
        }
        public async Task SavePublicAlgoAsync(PublicAlgoData data)
        {
            var enitites = data.ToEntity();

            await _table.InsertOrMergeAsync(enitites);
        }
        public async Task DeletePublicAlgoAsync(PublicAlgoData data)
        {
            var entities = data.ToEntity();
            await _table.DeleteAsync(entities);
        }
    }
}
