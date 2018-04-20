using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IUserPermissionsRepository
    {
        Task<List<UserPermissionData>> GetAllPermissionsAsync();
        Task<UserPermissionData> GetPermissionByIdAsync(string permissionId);
        Task<UserPermissionData> SavePermissionAsync(UserPermissionData permission);
        Task DeletePermissionAsync(UserPermissionData permission);
    }
}
