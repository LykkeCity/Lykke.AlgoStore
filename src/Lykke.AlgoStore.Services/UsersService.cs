using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.AlgoStore.Services.Strings;

namespace Lykke.AlgoStore.Services
{
    public class UsersService: BaseAlgoStoreService, IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly IAlgoStoreCommentsService _commentsService;
        private readonly ISecurityClient _securityClient;
        private readonly IAlgoInstancesService _instancesService;
        private readonly IAlgoInstanceStoppingClient _algoInstanceStoppingClient;
        private readonly IAlgosService _algosService;
        private readonly IAlgoBlobRepository _blobRepository;
        private readonly IAlgoStoreService _algoStoreService;
        private readonly IAlgoRepository _algoRepository;


        public UsersService(ILog log,
            IUsersRepository usersRepository,
            IAlgoStoreCommentsService commentsService,
            ISecurityClient securityClient,
            IAlgoInstancesService instancesService,
            IAlgoInstanceStoppingClient algoInstanceStoppingClient,
            IAlgosService algosService,
            IAlgoBlobRepository blobRepository,
            IAlgoStoreService algoStoreService,
            IAlgoClientInstanceRepository instanceRepository,
            IAlgoRepository algoRepository) : base(log, nameof(AlgoStoreService))
        {
            _usersRepository = usersRepository;
            _commentsService = commentsService;
            _securityClient = securityClient;
            _instancesService = instancesService;
            _algoInstanceStoppingClient = algoInstanceStoppingClient;
            _algosService = algosService;
            _blobRepository = blobRepository;
            _algoStoreService = algoStoreService;
            _instanceRepository = instanceRepository;
            _algoRepository = algoRepository;
        }

        public async Task SeedAsync(string clientId)
        {
            await LogTimedInfoAsync(nameof(SeedAsync), clientId, async () =>
            {
                var entity = await _usersRepository.GetByIdAsync(clientId);

                if (entity == null)
                {
                    entity = new UserData
                    {
                        ClientId = clientId,
                        CookieConsent = false,
                        GDPRConsent = false
                    };
                }

                await _usersRepository.UpdateAsync(entity);
            });
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

                if (entity == null)
                {
                    entity = new UserData
                    {
                        ClientId = clientId
                    };
                }

                if (entity.CookieConsent)
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

                if (entity == null)
                {
                    entity = new UserData
                    {
                        ClientId = clientId
                    };
                }

                if (entity.GDPRConsent)
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"GDPR consent already given for clientId {clientId}",
                        string.Format(Phrases.ConsentAlreadyGiven, "GDPR", clientId));
                }

                entity.GDPRConsent = true;

                await _usersRepository.UpdateAsync(entity);
            });
        }

        public async Task RemoveUserConsents(string clientId)
        {
            await LogTimedInfoAsync(nameof(RemoveUserConsents), clientId, async () =>
            {
                await _usersRepository.DeleteAsync(clientId);
            });
        }

        public async Task DeactivateAccountAsync(string clientId)
        {
            await LogTimedInfoAsync(nameof(DeactivateAccountAsync), clientId, async () =>
            {
                // first get all comments made by the user and unlink his id from author field
                var comments = await _commentsService.GetAllCommentsAsync();
                var userComments = comments.FindAll(c => c.Author == clientId);

                foreach(var comment in userComments)
                {
                    if (comment.Author == clientId)
                    {
                        comment.Author = null;
                        await _commentsService.UnlinkComment(comment);
                    }
                }

                // second get all roles for this user

                var user = await _securityClient.GetUserByIdWithRolesAsync(clientId);
                var roles = user.Roles;

                // delete his roles
                foreach (var role in roles)
                {
                    await _securityClient.RevokeRoleFromUserAsync(new Lykke.Service.Security.Client.AutorestClient.Models.UserRoleMatchModel()
                    {
                        ClientId = clientId,
                        RoleId = role.Id
                    });
                }

                // remove user legal consent
                await RemoveUserConsents(clientId);

                // find and stop all instances
                var instances = await _instanceRepository.GetAllAlgoInstancesByClientAsync(clientId);

                foreach (var instance in instances)
                {
                    var result = await _algoInstanceStoppingClient.DeleteAlgoInstanceAsync(instance.InstanceId, instance.AuthToken);
                }

                // replace author in algos
                var algos = await _algosService.GetAllUserAlgosAsync(clientId);

                foreach (var algo in algos)
                {
                    // update it
                    var algoToSave = AutoMapper.Mapper.Map<IAlgo>(algo);
                    algoToSave.ClientId = Guid.NewGuid().ToString();
                    algoToSave.DateModified = DateTime.Now;
                    await _algoRepository.SaveAlgoWithNewPKAsync(algoToSave, algo.ClientId);
                }
            });
        }        
    }
}
