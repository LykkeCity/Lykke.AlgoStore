using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class UserRolesRepository : IUserRolesRepository
    {
        public static readonly string TableName = "UserRoles";

        private readonly INoSQLTableStorage<UserRoleEntity> _table;

        public UserRolesRepository(INoSQLTableStorage<UserRoleEntity> table)
        {
            _table = table;
        }

        public async Task<List<UserRoleData>> GetAllRolesAsync()
        {
            var result = await _table.GetDataAsync();

            return AutoMapper.Mapper.Map<List<UserRoleData>>(result);
        }

        public async Task<UserRoleData> GetRoleByIdAsync(string roleId)
        {
            var result = await _table.GetDataAsync(roleId);
            return AutoMapper.Mapper.Map<UserRoleData>(result.ToList()[0]);
        }       

        public async Task<UserRoleData> SaveRoleAsync(UserRoleData role)
        {
            var entity = AutoMapper.Mapper.Map<UserRoleEntity>(role);

            await _table.InsertOrReplaceAsync(entity);
            return role;
        }

        public async Task DeleteRoleAsync(UserRoleData role)
        {
            await _table.DeleteAsync(role.Id, role.Name);
        }
    }
}
