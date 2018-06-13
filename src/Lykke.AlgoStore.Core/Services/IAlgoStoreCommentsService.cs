using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreCommentsService
    {
        Task<List<AlgoCommentData>> GetAlgoCommentsAsync(string algoId, string clientId);
        Task<AlgoCommentData> GetCommentByIdAsync(string algoId, string commentId, string clientId);
        Task<AlgoCommentData> SaveCommentAsync(AlgoCommentData data);
        Task<AlgoCommentData> EditCommentAsync(AlgoCommentData data);
        Task DeleteCommentAsync(string algoId, string commentId, string clientId);
    }
}
