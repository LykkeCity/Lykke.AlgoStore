using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoRuntimeDataRepository : IAlgoRuntimeDataRepository
    {
        public static readonly string TableName = "AlgoRuntimeDataTable";

        private const string PartitionKey = "AlgoRuntimeData";

        private readonly INoSQLTableStorage<AlgoRuntimeDataEntity> _table;

        public AlgoRuntimeDataRepository(INoSQLTableStorage<AlgoRuntimeDataEntity> table)
        {
            _table = table;
        }

        public async Task<AlgoClientRuntimeData> GetAlgoRuntimeDataByAlgo(string algoId)
        {
            var entities = await _table.GetDataAsync(PartitionKey, entity => entity.ClientAlgoId == algoId);

            return entities.ToList().ToModel();
        }

        public async Task SaveAlgoRuntimeData(AlgoClientRuntimeData data)
        {
            var enitites = data.ToEntity(PartitionKey);

            await _table.InsertOrMergeBatchAsync(enitites);
        }
        public async Task<bool> DeleteAlgoRuntimeData(string imageId)
        {
            var entity = await _table.DeleteAsync(PartitionKey, imageId);
            return entity != null;
        }
    }
}
