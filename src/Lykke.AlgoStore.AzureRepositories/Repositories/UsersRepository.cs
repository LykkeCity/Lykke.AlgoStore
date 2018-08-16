using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        public static readonly string TableName = "Users";
        private static readonly string PartitionKey = "User";

        private readonly INoSQLTableStorage<UserEntity> _table;

        public UsersRepository(INoSQLTableStorage<UserEntity> table)
        {
            _table = table;
        }

        public async Task SaveAsync(UserData data)
        {
            var entity = AutoMapper.Mapper.Map<UserEntity>(data);
            entity.PartitionKey = PartitionKey;
            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task<UserData> GetByIdAsync(string clientId)
        {
            var result = await _table.GetDataAsync(PartitionKey, clientId);
            return AutoMapper.Mapper.Map<UserData>(result);
        }

        public async Task UpdateAsync(UserData data)
        {
            var entity = AutoMapper.Mapper.Map<UserEntity>(data);
            entity.PartitionKey = PartitionKey;
            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task DeleteAsync(string clientId)
        {
            await _table.DeleteIfExistAsync(PartitionKey, clientId);
        }
    }
}
