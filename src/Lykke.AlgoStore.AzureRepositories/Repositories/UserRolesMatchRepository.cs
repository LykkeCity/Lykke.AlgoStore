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
    public class UserRolesMatchRepository : IUserRoleMatchRepository
    {
        public static readonly string TableName = "UserRolesMatch";

        private readonly INoSQLTableStorage<UserRoleMatchEntity> _table;

        public UserRolesMatchRepository(INoSQLTableStorage<UserRoleMatchEntity> table)
        {
            _table = table;
        }

        public async Task RevokeUserRole(string clientId, string roleId)
        {
            await _table.DeleteAsync(clientId, roleId);
        }

        public async Task<UserRoleMatchData> GetUserRoleAsync(string clientId, string roleId)
        {
            var result = await _table.GetDataAsync(clientId, roleId);
            return AutoMapper.Mapper.Map<UserRoleMatchData>(result);
        }

        public async Task<List<UserRoleMatchData>> GetUserRolesAsync(string clientId)
        {
            var result = await _table.GetDataAsync(clientId);
            return AutoMapper.Mapper.Map<List<UserRoleMatchData>>(result);
        }

        public async Task<UserRoleMatchData> SaveUserRoleAsync(UserRoleMatchData data)
        {
            var entity = AutoMapper.Mapper.Map<UserRoleMatchEntity>(data);
            await _table.InsertOrReplaceAsync(entity);

            return data;
        }
    }
}
