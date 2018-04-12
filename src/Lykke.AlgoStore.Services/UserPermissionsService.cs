using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services
{
    public class UserPermissionsService : BaseAlgoStoreService, IUserPermissionsService
    {
        private readonly IUserRolesRepository _rolesRepository;
        private readonly IUserPermissionsRepository _permissionsRepository;
        private readonly IRolePermissionMatchRepository _rolePermissionMatchRepository;

        public UserPermissionsService(IUserPermissionsRepository permissionsRepository,
            IRolePermissionMatchRepository rolePermissionMatchRepository,
            IUserRolesRepository rolesRepository,
             ILog log) : base(log, nameof(UserPermissionsService))
        {
            _permissionsRepository = permissionsRepository;
            _rolePermissionMatchRepository = rolePermissionMatchRepository;
            _rolesRepository = rolesRepository;
        }

        public async Task AssignPermissionToRoleAsync(RolePermissionMatchData data)
        {
            await LogTimedInfoAsync(nameof(AssignPermissionToRoleAsync), null, async () =>
            {
                if (string.IsNullOrEmpty(data.PermissionId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "PermissionId is empty.");

                if (string.IsNullOrEmpty(data.RoleId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "RoleId is empty.");

                var role = await _rolesRepository.GetRoleByIdAsync(data.RoleId);

                if (!role.CanBeModified)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "The permissions of this role cannot be modified.");

                await _rolePermissionMatchRepository.AssignPermissionToRoleAsync(data);
            });
        }

        public async Task<List<UserPermissionData>> GetAllPermissionsAsync()
        {
            return await LogTimedInfoAsync(nameof(GetAllPermissionsAsync), null, async () =>
            {
                var result = await _permissionsRepository.GetAllPermissionsAsync();
                return result;
            });
        }

        public async Task<UserPermissionData> GetPermissionByIdAsync(string permissionId)
        {
            return await LogTimedInfoAsync(nameof(GetPermissionByIdAsync), null, async () =>
            {
                if (string.IsNullOrEmpty(permissionId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "PermissionId is empty.");

                var result = await _permissionsRepository.GetPermissionByIdAsync(permissionId);
                return result;
            });
        }

        public async Task<List<UserPermissionData>> GetPermissionsByRoleIdAsync(string roleId)
        {
            return await LogTimedInfoAsync(nameof(GetPermissionsByRoleIdAsync), null, async () =>
            {
                if (string.IsNullOrEmpty(roleId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "RoleId is empty.");

                var matches = await _rolePermissionMatchRepository.GetPermissionIdsByRoleIdAsync(roleId);
                var permissions = new List<UserPermissionData>();

                for (var i = 0; i < matches.Count; i++)
                {
                    var permission = await GetPermissionByIdAsync(matches[i].PermissionId);
                    permissions.Add(permission);
                }

                return permissions;
            });
        }

        public async Task<UserPermissionData> SavePermissionAsync(UserPermissionData data)
        {
            return await LogTimedInfoAsync(nameof(SavePermissionAsync), null, async () =>
            {
                if (data.Id == null)
                {
                    data.Id = Guid.NewGuid().ToString();
                    data.DisplayName = Regex.Replace(data.Name, "([A-Z]{1,2}|[0-9]+)", " $1").TrimStart();
                }                    

                await _permissionsRepository.SavePermissionAsync(data);
                return data;
            });
        }        

        public async Task DeletePermissionAsync(string permissionId)
        {
            await LogTimedInfoAsync(nameof(DeletePermissionAsync), null, async () =>
            {
                if (string.IsNullOrEmpty(permissionId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "PermissionId is empty.");


                var permission = await GetPermissionByIdAsync(permissionId);
                await _permissionsRepository.DeletePermissionAsync(permission);
            });
        }

        public async Task RevokePermissionFromRole(RolePermissionMatchData data)
        {
            await LogTimedInfoAsync(nameof(RevokePermissionFromRole), null, async () =>
            {
                if (string.IsNullOrEmpty(data.RoleId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "RoleId is empty.");

                if (string.IsNullOrEmpty(data.PermissionId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "PermissionId is empty.");

                var role = await _rolesRepository.GetRoleByIdAsync(data.RoleId);

                if (!role.CanBeModified)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "The permissions of this role cannot be modified.");

                await _rolePermissionMatchRepository.RevokePermission(data);
            });
        }
    }
}
