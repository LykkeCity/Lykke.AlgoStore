using Lykke.AlgoStore.Core.Domain.Entities;
using System.Threading.Tasks;
using System;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IUsersService
    {
        Task<UserData> GetByIdAsync(string clientId);
        Task SetGDPRConsentAsync(string clientId);
        Task SetCookieConsentAsync(string clientId);
        Task DeactivateAccountAsync(string clientId);
    }
}
