using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;

namespace Lykke.AlgoStore.Services
{
    public class UsersService: IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public Task<UserData> GetByIdAsync(string clientId)
        {
            throw new NotImplementedException();
        }

        public Task SetCookieConsentAsync(string clientId, bool accepted = true)
        {
            throw new NotImplementedException();
        }

        public Task SetGDPRConsentAsync(string clientId, bool accepted = true)
        {
            throw new NotImplementedException();
        }

        public Task DeactivateAccountAsync(string clientId)
        {
            throw new NotImplementedException();
        }        
    }
}
