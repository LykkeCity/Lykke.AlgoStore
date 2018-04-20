using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IUserRoleMatchRepository
    {
        Task<List<UserRoleMatchData>> GetAllMatchesAsync();
        Task<UserRoleMatchData> GetUserRoleAsync(string clientId, string roleId);
        Task<List<UserRoleMatchData>> GetUserRolesAsync(string clientId);
        Task<UserRoleMatchData> SaveUserRoleAsync(UserRoleMatchData data);
        Task RevokeUserRole(string clientId, string roleId);
    }
}
