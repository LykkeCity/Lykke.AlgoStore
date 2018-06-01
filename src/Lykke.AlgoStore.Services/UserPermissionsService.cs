using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Services.Strings;
using Lykke.AlgoStore.Services.Utils;
using System;
using System.Collections.Generic;
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

        public async Task AssignPermissionsToRoleAsync(List<RolePermissionMatchData> data)
        {
            await LogTimedInfoAsync(nameof(AssignPermissionsToRoleAsync), null, async () =>
            {
                foreach (var permission in data)
                {
                    Check.IsEmpty(permission.PermissionId, nameof(permission.PermissionId));
                    Check.IsEmpty(permission.RoleId, nameof(permission.RoleId));

                    var dbPermission = await _permissionsRepository.GetPermissionByIdAsync(permission.PermissionId);

                    if (dbPermission == null)
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                            $"Permission with id {permission.PermissionId} does not exist.",
                            string.Format(Phrases.ParamNotFoundDisplayMessage, "permission"));

                    var role = await _rolesRepository.GetRoleByIdAsync(permission.RoleId);

                    if(role == null)
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"Role with id {permission.RoleId} does not exist.",
                            string.Format(Phrases.ParamNotFoundDisplayMessage, "role"));

                    if (!role.CanBeModified)
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.PermissionsCantBeModified,
                            Phrases.PermissionsCantBeModified);

                    await _rolePermissionMatchRepository.AssignPermissionToRoleAsync(permission);
                }               
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
                Check.IsEmpty(permissionId, nameof(permissionId));

                var result = await _permissionsRepository.GetPermissionByIdAsync(permissionId);
                return result;
            });
        }

        public async Task<List<UserPermissionData>> GetPermissionsByRoleIdAsync(string roleId)
        {
            return await LogTimedInfoAsync(nameof(GetPermissionsByRoleIdAsync), null, async () =>
            {
                Check.IsEmpty(roleId, nameof(roleId));

                var matches = await _rolePermissionMatchRepository.GetPermissionIdsByRoleIdAsync(roleId);
                var permissions = new List<UserPermissionData>();

                foreach (var match in matches)
                {
                    var permission = await GetPermissionByIdAsync(match.PermissionId);
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
                Check.IsEmpty(permissionId, nameof(permissionId));

                var permission = await GetPermissionByIdAsync(permissionId);

                if (permission == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "Permission with this ID does not exist",
                            string.Format(Phrases.ParamNotFoundDisplayMessage, "permission"));

                await _permissionsRepository.DeletePermissionAsync(permission);
            });
        }

        public async Task RevokePermissionsFromRole(List<RolePermissionMatchData> data)
        {
            await LogTimedInfoAsync(nameof(RevokePermissionsFromRole), null, async () =>
            {
                foreach (var permission in data)
                {
                    Check.IsEmpty(permission.RoleId, nameof(permission.RoleId));
                    Check.IsEmpty(permission.PermissionId, nameof(permission.PermissionId));

                    var role = await _rolesRepository.GetRoleByIdAsync(permission.RoleId);

                    if (!role.CanBeModified)
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.PermissionsCantBeModified,
                            Phrases.PermissionsCantBeModified);

                    await _rolePermissionMatchRepository.RevokePermission(permission);
                }                
            });
        }
    }
}
