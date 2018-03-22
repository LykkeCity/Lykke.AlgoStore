using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using System;

namespace Lykke.AlgoStore.AzureRepositories.Repositories
{
    public class AlgoCommentsRepository: IAlgoCommentsRepository
    {
        public static readonly string TableName = "AlgoComments";

        private readonly INoSQLTableStorage<AlgoCommentEntity> _table;

        public AlgoCommentsRepository(INoSQLTableStorage<AlgoCommentEntity> table)
        {
            _table = table;
        }

        public async Task<List<AlgoCommentData>> GetCommentsForAlgoAsync(string algoId)
        {
            var result = await _table.GetDataAsync(algoId);
            return result.ToList().ToModel();
        }

        public async Task<AlgoCommentData> GetCommentByIdAsync(string algoId, string commentId)
        {
            var result = await _table.GetDataAsync(algoId, commentId);
            return result?.ToModel();
        }

        public async Task<AlgoCommentData> SaveCommentAsync(AlgoCommentData data)
        {
            var entity = data.ToEntity();

            await _table.InsertOrReplaceAsync(entity);

            return entity.ToModel();
        }

        public async Task DeleteCommentAsync(string algoId, string commentId)
        {
            await _table.DeleteAsync(algoId, commentId);
        }

    }
}
