using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IRolePermissionMatchRepository
    {
        Task<List<RolePermissionMatchData>> GetPermissionIdsByRoleIdAsync(string roleId);
        Task<RolePermissionMatchData> AssignPermissionToRoleAsync(RolePermissionMatchData data);
        Task RevokePermission(RolePermissionMatchData data);
    }
}
