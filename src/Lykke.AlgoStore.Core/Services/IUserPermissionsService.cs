using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IUserPermissionsService
    {
        Task<List<UserPermissionData>> GetAllPermissionsAsync();
        Task<UserPermissionData> GetPermissionByIdAsync(string permissionId);
        Task<List<UserPermissionData>> GetPermissionsByRoleIdAsync(string roleId);
        Task<UserPermissionData> SavePermissionAsync(UserPermissionData data);
        Task RevokePermissionFromRole(RolePermissionMatchData data);
        Task RevokePermissionsFromRole(List<RolePermissionMatchData> data);
        Task AssignPermissionToRoleAsync(RolePermissionMatchData data);
        Task AssignPermissionsToRoleAsync(List<RolePermissionMatchData> data);
        Task DeletePermissionAsync(string permissionId);
    }
}
