using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IUserRolesRepository
    {
        Task<List<UserRoleData>> GetAllRolesAsync();
        Task<UserRoleData> GetRoleByIdAsync(string roleId);
        Task<UserRoleData> SaveRoleAsync(UserRoleData role);
        Task DeleteRoleAsync(UserRoleData data);
        Task<bool> RoleExistsAsync(string roleIdOrName);
    }
}
