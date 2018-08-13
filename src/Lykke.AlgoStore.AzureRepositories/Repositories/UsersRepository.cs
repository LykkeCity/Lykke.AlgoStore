using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        public static readonly string TableName = "Users";

        private readonly INoSQLTableStorage<UserEntity> _table;

        public UsersRepository(INoSQLTableStorage<UserEntity> table)
        {
            _table = table;
        }

        public Task<UserData> GetByIdAsync(string clientId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(string clientId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
