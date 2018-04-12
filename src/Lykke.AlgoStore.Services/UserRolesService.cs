using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services
{
    public class UserRolesService: BaseAlgoStoreService, IUserRolesService
    {
        private readonly IUserRolesRepository _rolesRepository;
        private readonly IUserPermissionsRepository _permissionsRepository;
        private readonly IUserRoleMatchRepository _userRoleMatchRepository;
        private readonly IUserPermissionsService _permissionsService;
        private readonly IRolePermissionMatchRepository _rolePermissionMatchRepository;

        public UserRolesService(IUserRolesRepository rolesRepository,
             IUserPermissionsRepository permissionsRepository,
             IUserRoleMatchRepository userRoleMatchRepository,
             IUserPermissionsService permissionsService,
             IRolePermissionMatchRepository rolePermissionMatchRepository,
             ILog log) : base(log, nameof(UserRolesService))
        {
            _rolesRepository = rolesRepository;
            _permissionsRepository = permissionsRepository;
            _userRoleMatchRepository = userRoleMatchRepository;
            _permissionsService = permissionsService;
            _rolePermissionMatchRepository = rolePermissionMatchRepository;
        }
        
        public async Task<List<UserRoleData>> GetAllRolesAsync()
        {
            return await LogTimedInfoAsync(nameof(GetAllRolesAsync), null, async () =>
            {
                var roles = await _rolesRepository.GetAllRolesAsync();

                for (var i = 0; i < roles.Count; i++)
                {
                    var permissionIds = await _rolePermissionMatchRepository.GetPermissionIdsByRoleIdAsync(roles[i].Id);

                    var permissions = new List<UserPermissionData>();
                    for (var j = 0; j < permissionIds.Count; j++)
                    {
                        var perm = await _permissionsRepository.GetPermissionByIdAsync(permissionIds[j].PermissionId);
                        permissions.Add(perm);
                    }

                    roles[i].Permissions = permissions;

                }

                return roles;
            });
        }

        public async Task<UserRoleData> GetRoleByIdAsync(string roleId)
        {
            return await LogTimedInfoAsync(nameof(GetRoleByIdAsync), null, async () =>
            {
                if (string.IsNullOrEmpty(roleId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "RoleId is empty.");

                var role = await _rolesRepository.GetRoleByIdAsync(roleId);
                var permissionIds = await _rolePermissionMatchRepository.GetPermissionIdsByRoleIdAsync(roleId);

                var permissions = new List<UserPermissionData>();
                for (var i = 0; i < permissionIds.Count; i++)
                {
                    var perm = await _permissionsRepository.GetPermissionByIdAsync(permissionIds[i].PermissionId);
                    permissions.Add(perm);
                }

                role.Permissions = permissions;
                return role;
            });
        }

        public async Task<List<UserRoleData>> GetRolesByClientIdAsync(string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetRolesByClientIdAsync), clientId, async () =>
            {
                if (string.IsNullOrEmpty(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId is empty.");

                var roleMatches = await _userRoleMatchRepository.GetUserRolesAsync(clientId);
                var roles = new List<UserRoleData>();
                for (var i = 0; i < roleMatches.Count; i++)
                {
                    var role = await GetRoleByIdAsync(roleMatches[i].RoleId);
                    roles.Add(role);
                }

                return roles;
            });
        }

        public async Task AssignRoleToUser(UserRoleMatchData data)
        {
            await LogTimedInfoAsync(nameof(AssignRoleToUser), data.ClientId, async () =>
            {
                if (string.IsNullOrEmpty(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId is empty.");

                if (string.IsNullOrEmpty(data.RoleId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "RoleId is empty.");

                await _userRoleMatchRepository.SaveUserRoleAsync(data);
            });
        }

        public async Task<UserRoleData> SaveRoleAsync(UserRoleData role)
        {
            return await LogTimedInfoAsync(nameof(SaveRoleAsync), null, async () =>
            {
                if (role.Id == null)
                {
                    role.Id = Guid.NewGuid().ToString();
                    role.CanBeModified = true;
                    role.CanBeDeleted = true;
                }                    
                else
                {
                    var dbRole = await _rolesRepository.GetRoleByIdAsync(role.Id);
                    if(dbRole != null && !dbRole.CanBeModified)
                    {
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "This role can't be modified.");
                    }
                }
                

                await _rolesRepository.SaveRoleAsync(role);
                return role;
            });
        }

        public async Task RevokeRoleFromUser(UserRoleMatchData data)
        {
            await LogTimedInfoAsync(nameof(RevokeRoleFromUser), data.ClientId, async () =>
            {
                if (string.IsNullOrEmpty(data.RoleId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "RoleId is empty.");

                if (string.IsNullOrEmpty(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId is empty.");

                await _userRoleMatchRepository.RevokeUserRole(data.ClientId, data.RoleId);
            });
        }


        public async Task VerifyUserRole(string clientId)
        {
            await LogTimedInfoAsync(nameof(VerifyUserRole), clientId, async () =>
            {
                if (string.IsNullOrEmpty(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId is empty.");


                var roles = await _userRoleMatchRepository.GetUserRolesAsync(clientId);

                if (roles.Count == 0)
                {
                    var newRole = await _userRoleMatchRepository.SaveUserRoleAsync(new UserRoleMatchData()
                    {
                        RoleId = "User",
                        ClientId = clientId
                    });
                }
            });
        }

        public async Task DeleteRoleAsync(string roleId)
        {
            await LogTimedInfoAsync(nameof(DeleteRoleAsync), null, async () =>
            {
                if (string.IsNullOrEmpty(roleId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "RoleId is empty.");

                //first check if the role has permissions assigned
                var permissionsForRole = await _rolePermissionMatchRepository.GetPermissionIdsByRoleIdAsync(roleId);
                for (var i = 0; i < permissionsForRole.Count; i++)
                {
                    // if it does, delete them
                    await _rolePermissionMatchRepository.RevokePermission(permissionsForRole[i]);
                }              

                var role = await _rolesRepository.GetRoleByIdAsync(roleId);

                if(!role.CanBeDeleted)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "This role cannot be deleted.");

                await _rolesRepository.DeleteRoleAsync(role);
            });
        }
    }
}
