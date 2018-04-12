using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class RolePermissionMatchRepository : IRolePermissionMatchRepository
    {

        public static readonly string TableName = "RolePermissionMatch";

        private readonly INoSQLTableStorage<RolePermissionMatchEntity> _table;

        public RolePermissionMatchRepository(INoSQLTableStorage<RolePermissionMatchEntity> table)
        {
            _table = table;
        }

        public async Task<RolePermissionMatchData> AssignPermissionToRoleAsync(RolePermissionMatchData data)
        {
            var entity = data.ToEntity();
            await _table.InsertOrReplaceAsync(entity);

            return data;
        }

        public async Task<List<RolePermissionMatchData>> GetPermissionIdsByRoleIdAsync(string roleId)
        {
            var result = await _table.GetDataAsync(roleId);

            return result.ToList().ToModel();
        }

        public async Task RevokePermission(RolePermissionMatchData data)
        {
            await _table.DeleteAsync(data.RoleId, data.PermissionId);
        }
    }
}
