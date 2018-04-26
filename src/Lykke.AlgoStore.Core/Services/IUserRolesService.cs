using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IUserRolesService
    {
        Task<List<UserRoleData>> GetAllRolesAsync();
        Task<UserRoleData> GetRoleByIdAsync(string roleId);
        Task<List<UserRoleData>> GetRolesByClientIdAsync(string clientId);
        Task<List<AlgoStoreUserData>> GetAllUsersWithRolesAsync();
        Task<AlgoStoreUserData> GeyUserByIdWithRoles(string clientId);
        Task AssignRoleToUser(UserRoleMatchData data);
        Task<UserRoleData> SaveRoleAsync(UserRoleData role);
        Task RevokeRoleFromUser(UserRoleMatchData data);
        Task VerifyUserRole(string clientId);
        Task DeleteRoleAsync(string roleId);
    }
}
