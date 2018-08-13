using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IUsersRepository
    {
        Task<UserData> GetByIdAsync(string clientId);
        Task SaveAsync(UserData data);
        Task UpdateAsync(UserData data);
        Task DeleteAsync(string clientId);
    }
}
