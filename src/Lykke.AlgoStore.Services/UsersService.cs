using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Services.Strings;

namespace Lykke.AlgoStore.Services
{
    public class UsersService: BaseAlgoStoreService, IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(ILog log, IUsersRepository usersRepository): base(log, nameof(AlgoStoreService))
        {
            _usersRepository = usersRepository;
        }

        public async Task<UserData> GetByIdAsync(string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetByIdAsync), clientId, async () =>
            {
                var result = await _usersRepository.GetByIdAsync(clientId);

                return result;
            });
        }

        public async Task SetCookieConsentAsync(string clientId)
        {
            await LogTimedInfoAsync(nameof(SetCookieConsentAsync), clientId, async () =>
            {
                var entity = await _usersRepository.GetByIdAsync(clientId);

                if(entity.CookieConsent)
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"Cookie consent already given for clientId {clientId}",
                        string.Format(Phrases.ConsentAlreadyGiven, "Cookie", clientId));
                }

                entity.CookieConsent = true;

                await _usersRepository.UpdateAsync(entity);
            });
        }

        public async Task SetGDPRConsentAsync(string clientId)
        {
            await LogTimedInfoAsync(nameof(SetGDPRConsentAsync), clientId, async () =>
            {
                var entity = await _usersRepository.GetByIdAsync(clientId);

                if (entity.GDPRConsent)
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"GDPR consent already given for clientId {clientId}",
                        string.Format(Phrases.ConsentAlreadyGiven, "GDPR", clientId));
                }

                entity.GDPRConsent = true;

                await _usersRepository.UpdateAsync(entity);
            });
        }

        public Task DeactivateAccountAsync(string clientId)
        {
            throw new NotImplementedException();
        }        
    }
}
