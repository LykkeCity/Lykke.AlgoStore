using System.Collections;
using System.Collections.Generic;
using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoRepository : IAlgoRepository
    {
        public static readonly string TableName = "AlgoMetaDataTable";

        private readonly INoSQLTableStorage<AlgoEntity> _table;

        public AlgoRepository(INoSQLTableStorage<AlgoEntity> table)
        {
            _table = table;
        }


        public async Task<IEnumerable<IAlgo>> GetAllAlgosAsync()
        {
            var result = await _table.GetDataAsync();
            return result.ToList();
        }

        public async Task<IEnumerable<IAlgo>> GetAllClientAlgosAsync(string clientId)
        {
            var entities = await _table.GetDataAsync(clientId);
            return entities.OrderBy(a => a.Name);
        }

        public async Task<IAlgo> GetAlgoAsync(string clientId, string algoId)
        {
            var entitiy = await _table.GetDataAsync(clientId, algoId);

            if (entitiy == null)
                return null;

            return entitiy;
        }

        public async Task<AlgoClientMetaDataInformation> GetAlgoMetaDataInformationAsync(string clientId, string algoId)
        {
            var entitiy = await _table.GetDataAsync(clientId, algoId);

            return entitiy?.ToAlgoMetaInformation();
        }

        public async Task<bool> ExistsAlgoMetaDataAsync(string clientId, string algoId)
        {
            var entity = new AlgoEntity
            {
                PartitionKey = clientId,
                RowKey = algoId
            };

            return await _table.RecordExistsAsync(entity);
        }

        public async Task SaveAlgoAsync(IAlgo algo)
        {
            var enitity = AlgoEntity.Create(algo);
            await _table.InsertOrMergeAsync(enitity);
        }

        public async Task DeleteAlgoAsync(string clientId, string algoId)
        {
            await _table.DeleteAsync(clientId, algoId);
        }
    }
}
