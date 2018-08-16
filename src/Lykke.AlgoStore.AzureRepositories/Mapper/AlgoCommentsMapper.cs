using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using System.Collections.Generic;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoCommentsMapper
    {
        public static AlgoCommentData ToModel(this AlgoCommentEntity entity)
        {
            var result = new AlgoCommentData
            {
                AlgoId = entity.PartitionKey,
                CommentId = entity.RowKey,
                Author = entity.AuthorId,
                Content = entity.Content,
                CreatedOn = entity.CreatedOn,
                EditedOn = entity.EditedOn
            };

            return result;
        }

        public static List<AlgoCommentData> ToModel(this List<AlgoCommentEntity> entities)
        {
            var result = new List<AlgoCommentData>();

            foreach (var entity in entities)
            {
                var data = new AlgoCommentData
                {
                    AlgoId = entity.PartitionKey,
                    CommentId = entity.RowKey,
                    Author = entity.AuthorId,
                    Content = entity.Content,
                    CreatedOn = entity.CreatedOn,
                    EditedOn = entity.EditedOn
                };

                result.Add(data);
            }            

            return result;
        }

        public static List<AlgoCommentEntity> ToEntity(this List<AlgoCommentData> entities)
        {
            var result = new List<AlgoCommentEntity>();

            foreach (var entity in entities)
            {
                var data = new AlgoCommentEntity
                {
                    PartitionKey = entity.AlgoId,
                    RowKey = entity.CommentId,
                    AuthorId = entity.Author,
                    Content = entity.Content,
                    CreatedOn = entity.CreatedOn,
                    EditedOn = entity.EditedOn
                };

                result.Add(data);
            }

            return result;
        }

        public static AlgoCommentEntity ToEntity(this AlgoCommentData data)
        {
            var result = new AlgoCommentEntity
            {
                PartitionKey = data.AlgoId,
                RowKey = data.CommentId,
                AuthorId = data.Author,
                Content = data.Content,
                CreatedOn = data.CreatedOn,
                EditedOn = data.EditedOn
            };

            return result;
        }
    }
}
