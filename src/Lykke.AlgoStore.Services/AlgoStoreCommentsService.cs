using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Services.Strings;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Service.PersonalData.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreCommentsService : BaseAlgoStoreService, IAlgoStoreCommentsService
    {

        private readonly IAlgoCommentsRepository _algoCommentsRepository;
        private readonly IPersonalDataService _personalDataService;

        public AlgoStoreCommentsService(IAlgoCommentsRepository algoCommentsRepository,
            IPersonalDataService personalDataService,
            ILog log) : base(log, nameof(AlgosService))
        {
            _algoCommentsRepository = algoCommentsRepository;
            _personalDataService = personalDataService;
        }

        public async Task<List<AlgoCommentData>> GetAllCommentsAsync()
        {
            return await LogTimedInfoAsync(nameof(GetAllCommentsAsync), null, async () =>
            {   
                var result = await _algoCommentsRepository.GetAllAsync();               

                return result;
            });
        }

        public async Task<List<AlgoCommentData>> GetAlgoCommentsAsync(string algoId, string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoCommentsAsync), clientId, async () =>
             {
                 Check.IsEmpty(algoId, nameof(algoId));
                 Check.IsEmpty(clientId, nameof(clientId));

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

                 result.Sort((x, y) => DateTime.Compare(y.CreatedOn, x.CreatedOn));

                 return result;
             });
        }

        public async Task<AlgoCommentData> GetCommentByIdAsync(string algoId, string commentId, string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetCommentByIdAsync), clientId, async () =>
            {
                Check.IsEmpty(clientId, nameof(clientId));
                Check.IsEmpty(algoId, nameof(algoId));

                var result = await _algoCommentsRepository.GetCommentByIdAsync(algoId, commentId);                

                if(result != null)
                {
                    if (String.IsNullOrEmpty(result.Author))
                        result.Author = "Administrator";
                    else
                    {
                        var authorPersonalData = await _personalDataService.GetAsync(result.Author);
                        result.Author = !String.IsNullOrEmpty(authorPersonalData.FullName)
                                                                ? authorPersonalData.FullName
                                                                : authorPersonalData.Email;
                    }
                }
                
                return result;
            });
        }

        public async Task<AlgoCommentData> SaveCommentAsync(AlgoCommentData data)
        {
            return await LogTimedInfoAsync(nameof(SaveCommentAsync), data.Author, async () =>
            {
                Check.IsEmpty(data.Author, "ClientID");
                Check.IsEmpty(data.AlgoId, nameof(data.AlgoId));
                
                data.CommentId = Guid.NewGuid().ToString();
                data.CreatedOn = DateTime.UtcNow;                   

                var result = await _algoCommentsRepository.SaveCommentAsync(data);

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

        public async Task UnlinkComment(AlgoCommentData data)
        {
            await LogTimedInfoAsync(nameof(UnlinkComment), null, async () =>
            {
                await _algoCommentsRepository.SaveCommentAsync(data);
            });
        }

        public async Task<AlgoCommentData> EditCommentAsync(AlgoCommentData data)
        {
            return await LogTimedInfoAsync(nameof(EditCommentAsync), data.Author, async () =>
            {
                Check.IsEmpty(data.CommentId, nameof(data.CommentId));
                Check.IsEmpty(data.AlgoId, nameof(data.AlgoId));

                var comment = await _algoCommentsRepository.GetCommentByIdAsync(data.AlgoId, data.CommentId);

                if(comment == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, 
                        "Comment with this CommentId and AlgoId does not exist.",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "comment"));

                comment.EditedOn = DateTime.UtcNow;
                comment.Content = data.Content;

                var result = await _algoCommentsRepository.SaveCommentAsync(comment);

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

        public async Task DeleteCommentAsync(string algoId, string commentId, string clientId)
        {
            await LogTimedInfoAsync(nameof(GetCommentByIdAsync), clientId, async () =>
            {
                Check.IsEmpty(commentId, nameof(commentId));
                Check.IsEmpty(algoId, nameof(algoId));

                await _algoCommentsRepository.DeleteCommentAsync(algoId, commentId);
            });
        }
    }
}
