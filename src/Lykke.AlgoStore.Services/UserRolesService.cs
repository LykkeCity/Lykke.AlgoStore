using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.Service.PersonalData.Contract;

namespace Lykke.AlgoStore.Services
{
    public class UserRolesService : BaseAlgoStoreService, IUserRolesService
    {
        private readonly IUserRolesRepository _rolesRepository;
        private readonly IUserPermissionsRepository _permissionsRepository;
        private readonly IUserRoleMatchRepository _userRoleMatchRepository;
        private readonly IRolePermissionMatchRepository _rolePermissionMatchRepository;
        private readonly IPersonalDataService _personalDataService;

        public UserRolesService(IUserRolesRepository rolesRepository,
            IUserPermissionsRepository permissionsRepository,
            IUserRoleMatchRepository userRoleMatchRepository,
            IRolePermissionMatchRepository rolePermissionMatchRepository,
            IPersonalDataService personalDataService,
            ILog log) : base(log, nameof(UserRolesService))
        {
            _rolesRepository = rolesRepository;
            _permissionsRepository = permissionsRepository;
            _userRoleMatchRepository = userRoleMatchRepository;
            _rolePermissionMatchRepository = rolePermissionMatchRepository;
            _personalDataService = personalDataService;
        }

        public async Task<List<UserRoleData>> GetAllRolesAsync()
        {
            return await LogTimedInfoAsync(nameof(GetAllRolesAsync), null, async () =>
            {
                var roles = await _rolesRepository.GetAllRolesAsync();

                foreach (var role in roles)
                {
                    var permissionIds = await _rolePermissionMatchRepository.GetPermissionIdsByRoleIdAsync(role.Id);

                    var permissions = new List<UserPermissionData>();
                    foreach (var permission in permissionIds)
                    {
                        var perm = await _permissionsRepository.GetPermissionByIdAsync(permission.PermissionId);
                        permissions.Add(perm);
                    }

                    role.Permissions = permissions;
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

                if (role == null)
                    return null;

                var permissionIds = await _rolePermissionMatchRepository.GetPermissionIdsByRoleIdAsync(roleId);

                var permissions = new List<UserPermissionData>();
                foreach (var permission in permissionIds)
                {
                    var perm = await _permissionsRepository.GetPermissionByIdAsync(permission.PermissionId);
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
                foreach (var roleMatch in roleMatches)
                {
                    var role = await GetRoleByIdAsync(roleMatch.RoleId);
                    roles.Add(role);
                }

                return roles;
            });
        }

        public async Task<List<AlgoStoreUserData>> GetAllUsersWithRolesAsync()
        {
            return await LogTimedInfoAsync(nameof(GetAllUsersWithRolesAsync), null, async () =>
            {
                var result = new List<AlgoStoreUserData>();
                var matches = await _userRoleMatchRepository.GetAllMatchesAsync();
                var groupedClientIds = matches.GroupBy(m => m.ClientId).ToList();

                foreach (var item in groupedClientIds)
                {
                    var data = new AlgoStoreUserData();
                    var personalInformation = await _personalDataService.GetAsync(item.Key);
                    data.ClientId = item.Key;
                    data.FirstName = personalInformation?.FirstName;
                    data.LastName = personalInformation?.LastName;
                    data.FullName = personalInformation?.FullName;
                    data.Email = personalInformation?.Email;

                    data.Roles = item.Select(match => _rolesRepository.GetRoleByIdAsync(match.RoleId).Result).ToList();

                    result.Add(data);
                }

                return result;
            });
        }

        public async Task<AlgoStoreUserData> GeyUserByIdWithRoles(string clientId)
        {
            return await LogTimedInfoAsync(nameof(GeyUserByIdWithRoles), clientId, async () =>
            {
                if (string.IsNullOrEmpty(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId is empty.");

                var matches = await _userRoleMatchRepository.GetUserRolesAsync(clientId);

                var data = new AlgoStoreUserData();
                var personalInformation = await _personalDataService.GetAsync(clientId);
                data.ClientId = clientId;
                data.FirstName = personalInformation?.FirstName;
                data.LastName = personalInformation?.LastName;
                data.FullName = personalInformation?.FullName;
                data.Email = personalInformation?.Email;

                data.Roles = matches.Select(match => _rolesRepository.GetRoleByIdAsync(match.RoleId).Result).ToList();

                return data;
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

                var role = await _rolesRepository.GetRoleByIdAsync(data.RoleId);

                if (role == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        $"Role with id {data.RoleId} does not exist.");

                var clientData = await _personalDataService.GetAsync(data.ClientId);

                if (clientData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        $"Client with id {data.ClientId} does not exist.");

                await _userRoleMatchRepository.SaveUserRoleAsync(data);
            });
        }

        public async Task<UserRoleData> SaveRoleAsync(UserRoleData role)
        {
            return await LogTimedInfoAsync(nameof(SaveRoleAsync), null, async () =>
            {
                if (!String.IsNullOrEmpty(role.Name) && await _rolesRepository.RoleExistsAsync(role.Name))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        $"Role {role.Name} already exists.");

                if (role.Id == null)
                {
                    role.Id = Guid.NewGuid().ToString();
                    role.CanBeModified = true;
                    role.CanBeDeleted = true;
                }
                else
                {
                    var dbRole = await _rolesRepository.GetRoleByIdAsync(role.Id);
                    if (dbRole != null && !dbRole.CanBeModified)
                    {
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                            "This role can't be modified.");
                    }

                    // because the RK is the role name, in order to update it, first delete the old role and then replace it with the new one
                    await _rolesRepository.DeleteRoleAsync(dbRole);
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
                    var allRoles = await _rolesRepository.GetAllRolesAsync();

                    // original user role cannot be deleted
                    var userRole = allRoles.FirstOrDefault(role => role.Name == "User" && !role.CanBeDeleted);

                    if (userRole == null)
                        throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                            "Current user does not belong to 'User' role.");

                    await _userRoleMatchRepository.SaveUserRoleAsync(new UserRoleMatchData
                    {
                        RoleId = userRole.Id,
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

                var role = await _rolesRepository.GetRoleByIdAsync(roleId);

                if (role == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        $"Role with id {roleId} does not exist.");

                //first check if the role has permissions assigned
                var permissionsForRole = await _rolePermissionMatchRepository.GetPermissionIdsByRoleIdAsync(roleId);
                foreach (var permissionForRole in permissionsForRole)
                {
                    // if it does, delete them
                    await _rolePermissionMatchRepository.RevokePermission(permissionForRole);
                }

                //then check if any user is assigned to this role
                var allMatches = await _userRoleMatchRepository.GetAllMatchesAsync();
                var usersWithRole = allMatches.Where(m => m.RoleId == roleId).ToList();

                if (usersWithRole.Count > 0)
                {
                    // it there are any, revoke it
                    foreach (var match in usersWithRole)
                    {
                        await _userRoleMatchRepository.RevokeUserRole(match.ClientId, match.RoleId);
                    }
                }

                if (!role.CanBeDeleted)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "This role cannot be deleted.");

                await _rolesRepository.DeleteRoleAsync(role);
            });
        }
    }
}
