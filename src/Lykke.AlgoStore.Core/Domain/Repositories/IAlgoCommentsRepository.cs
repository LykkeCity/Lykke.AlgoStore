using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoCommentsRepository
    {
        Task<List<AlgoCommentData>> GetCommentsForAlgoAsync(string algoId);
        Task<AlgoCommentData> GetCommentByIdAsync(string algoId, string commentId);
        Task<AlgoCommentData> SaveCommentAsync(AlgoCommentData data);
        Task DeleteCommentAsync(string algoId, string commentId);
    }
}
