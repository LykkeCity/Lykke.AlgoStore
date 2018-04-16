using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.AzureRepositories.Mapper;
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
            return result.ToList().ToModel();
        }

        public async Task<UserPermissionData> GetPermissionByIdAsync(string permissionId)
        {
            var result = await _table.GetDataAsync(permissionId);
            return result.FirstOrDefault()?.ToModel();
        }

        public async Task<UserPermissionData> SavePermissionAsync(UserPermissionData permission)
        {
            var entity = permission.ToEntity();
            await _table.InsertOrReplaceAsync(entity);

            return permission;
        }

        public async Task DeletePermissionAsync(UserPermissionData permission)
        {
            var entity = permission.ToEntity();
            await _table.DeleteIfExistAsync(entity.PartitionKey, entity.RowKey);
        }
    }
}
