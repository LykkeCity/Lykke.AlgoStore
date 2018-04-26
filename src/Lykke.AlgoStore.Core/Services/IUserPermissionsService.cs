﻿using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IUserPermissionsService
    {
        Task<List<UserPermissionData>> GetAllPermissionsAsync();
        Task<UserPermissionData> GetPermissionByIdAsync(string permissionId);
        Task<List<UserPermissionData>> GetPermissionsByRoleIdAsync(string roleId);
        Task<UserPermissionData> SavePermissionAsync(UserPermissionData data);
        Task RevokePermissionsFromRole(List<RolePermissionMatchData> data);
        Task AssignPermissionsToRoleAsync(List<RolePermissionMatchData> data);
        Task DeletePermissionAsync(string permissionId);
    }
}
