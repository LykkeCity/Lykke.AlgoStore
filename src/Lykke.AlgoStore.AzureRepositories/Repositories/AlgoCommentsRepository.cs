using AzureStorage;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Microsoft.WindowsAzure.Storage.Table;

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

        public async Task<List<AlgoCommentData>> GetAllAsync()
        {
            var result = new List<AlgoCommentEntity>();
            var query = new TableQuery<AlgoCommentEntity>()
                .Take(1000);

            await _table.ExecuteAsync(query, (comments) =>
            {
                result.AddRange(comments);

            }, () => true);

            return result?.ToList().ToModel();
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

        public async Task DeleteCommentsAsync(string algoId)
        {
            var query = new TableQuery<AlgoCommentEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, algoId))
                .Take(100);

            await _table.ExecuteAsync(query, async (ratings) =>
            {
                var tableBatchOperation = new TableBatchOperation();

                foreach (var rating in ratings)
                {
                    tableBatchOperation.Delete(rating);
                }

                await _table.DoBatchAsync(tableBatchOperation);
            }, () => true);
        }

        public async Task DeleteCommentAsync(string algoId, string commentId)
        {
            await _table.DeleteAsync(algoId, commentId);
        }

    }
}
