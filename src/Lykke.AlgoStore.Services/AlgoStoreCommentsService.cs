using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.Service.PersonalData.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreCommentsService : BaseAlgoStoreService, IAlgoStoreCommentsService
    {

        private readonly IAlgoCommentsRepository _algoCommentsRepository;
        private readonly IPersonalDataService _personalDataService;

        public AlgoStoreCommentsService(IAlgoCommentsRepository algoCommentsRepository,
            IPersonalDataService personalDataService,
            ILog log) : base(log, nameof(AlgoStoreClientDataService))
        {
            _algoCommentsRepository = algoCommentsRepository;
            _personalDataService = personalDataService;
        }

        public async Task<List<AlgoCommentData>> GetAlgoCommentsAsync(string algoId, string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoCommentsAsync), clientId, async () =>
             {
                 if (string.IsNullOrEmpty(clientId))
                     throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientID is empty.");

                 if (string.IsNullOrEmpty(algoId))
                     throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId is empty.");

                 var result = await _algoCommentsRepository.GetCommentsForAlgoAsync(algoId);

                 foreach (var comment in result)
                 {
                     if (String.IsNullOrEmpty(comment.Author))
                         comment.Author = "Administrator";
                     else
                     {
                         var authorPersonalData = await _personalDataService.GetAsync(comment.Author);
                         comment.Author = !String.IsNullOrEmpty(authorPersonalData.FullName)
                                                                 ? authorPersonalData.FullName
                                                                : authorPersonalData.Email;
                     }                     
                 }

                 return result;
             });
        }

        public async Task<AlgoCommentData> GetCommentByIdAsync(string algoId, string commentId, string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetCommentByIdAsync), clientId, async () =>
            {
                if (string.IsNullOrEmpty(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientID is empty.");

                if (string.IsNullOrEmpty(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId is empty.");

                var result = await _algoCommentsRepository.GetCommentByIdAsync(algoId, commentId);                

                if (String.IsNullOrEmpty(result.Author))
                    result.Author = "Administrator";
                else
                {
                    var authorPersonalData = await _personalDataService.GetAsync(result.Author);
                    result.Author = !String.IsNullOrEmpty(authorPersonalData.FullName)
                                                            ? authorPersonalData.FullName
                                                            : authorPersonalData.Email;
                }
                return result;
            });
        }

        public async Task<AlgoCommentData> SaveCommentAsync(AlgoCommentData data)
        {
            return await LogTimedInfoAsync(nameof(GetCommentByIdAsync), data.Author, async () =>
            {
                if (string.IsNullOrEmpty(data.Author))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientID is empty.");

                if (string.IsNullOrEmpty(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId is empty.");

                
                data.CommentId = Guid.NewGuid().ToString();
                data.CreatedOn = DateTime.Now;                   

                var result = await _algoCommentsRepository.SaveCommentAsync(data);
                return result;
            });
        }

        public async Task<AlgoCommentData> EditCommentAsync(AlgoCommentData data)
        {
            return await LogTimedInfoAsync(nameof(EditCommentAsync), data.Author, async () =>
            {
                if (string.IsNullOrEmpty(data.Author))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientID is empty.");

                if (string.IsNullOrEmpty(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId is empty.");

                var comment = await _algoCommentsRepository.GetCommentByIdAsync(data.AlgoId, data.CommentId);

                comment.EditedOn = DateTime.Now;
                comment.Content = data.Content;

                var result = await _algoCommentsRepository.SaveCommentAsync(comment);
                return result;
            });
        }

        public async Task DeleteCommentAsync(string algoId, string commentId, string clientId)
        {
            await LogTimedInfoAsync(nameof(GetCommentByIdAsync), clientId, async () =>
            {
                if (string.IsNullOrEmpty(commentId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "CommentId is empty.");

                if (string.IsNullOrEmpty(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId is empty.");

                await _algoCommentsRepository.DeleteCommentAsync(algoId, commentId);
            });
        }
    }
}
