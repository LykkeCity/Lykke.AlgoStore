using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.AlgoStore.AzureRepositories.Mapper;

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

            return result.ToList().ToModel();
        }

        public async Task<UserRoleData> GetRoleByIdAsync(string roleId)
        {
            var result = await _table.GetDataAsync(roleId);
            if(result.ToList().Count > 0)
             return result.ToList()[0].ToModel();

            return null;
        }       

        public async Task<UserRoleData> SaveRoleAsync(UserRoleData role)
        {
            var entity = role.ToEntity();

            await _table.InsertOrReplaceAsync(entity);
            return role;
        }

        public async Task DeleteRoleAsync(UserRoleData role)
        {
            await _table.DeleteIfExistAsync(role.Id, role.Name);
        }
    }
}
