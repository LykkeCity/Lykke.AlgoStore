using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class UserPermissionsRepository: IUserPermissionsRepository
    {
        public static readonly string TableName = "UserPermissions";

        private readonly INoSQLTableStorage<UserPermissionEntity> _table;

        public UserPermissionsRepository(INoSQLTableStorage<UserPermissionEntity> table)
        {
            _table = table;
        }

        public async Task<List<UserPermissionData>> GetAllPermissionsAsync()
        {
            var result = await _table.GetDataAsync();
            return AutoMapper.Mapper.Map<List<UserPermissionData>>(result);
        }

        public async Task<UserPermissionData> GetPermissionByIdAsync(string permissionId)
        {
            var result = await _table.GetDataAsync(permissionId);
            return AutoMapper.Mapper.Map<UserPermissionData>(result.ToList()[0]);
        }

        public async Task<UserPermissionData> SavePermissionAsync(UserPermissionData permission)
        {
            var entity = AutoMapper.Mapper.Map<UserPermissionEntity>(permission);
            await _table.InsertOrReplaceAsync(entity);

            return permission;
        }

        public async Task DeletePermissionAsync(UserPermissionData permission)
        {
            var entity = AutoMapper.Mapper.Map<UserPermissionEntity>(permission);
            await _table.DeleteAsync(entity);
        }
    }
}
