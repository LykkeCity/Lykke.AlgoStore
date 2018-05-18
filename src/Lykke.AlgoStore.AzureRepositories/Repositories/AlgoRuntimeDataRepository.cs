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

        private readonly INoSQLTableStorage<AlgoRuntimeDataEntity> _table;

        public AlgoRuntimeDataRepository(INoSQLTableStorage<AlgoRuntimeDataEntity> table)
        {
            _table = table;
        }

        public async Task<AlgoClientRuntimeData> GetAlgoRuntimeDataAsync(string clientId, string algoId)
        {
            var entities = await _table.GetDataAsync(clientId, algoId);

            return entities.ToModel();
        }

        public async Task SaveAlgoRuntimeDataAsync(AlgoClientRuntimeData data)
        {
            var enitites = data.ToEntity();

            await _table.InsertOrMergeAsync(enitites);
        }
        public async Task<bool> DeleteAlgoRuntimeDataAsync(string clientId, string algoId)
        {
            var entity = await _table.DeleteAsync(clientId, algoId);
            return entity != null;
        }
    }
}
