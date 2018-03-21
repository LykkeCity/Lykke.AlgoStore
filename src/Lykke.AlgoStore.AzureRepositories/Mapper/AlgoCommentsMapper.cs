using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoCommentsMapper
    {
        public static AlgoCommentData ToModel(this AlgoCommentEntity entity)
        {
            var result = new AlgoCommentData();
            result.AlgoId = entity.PartitionKey;
            result.CommentId = entity.RowKey;
            result.Author = entity.AutorId;
            result.Content = entity.Content;
            result.CreatedOn = entity.CreatedOn;
            result.EditedOn = entity.EditedOn;

            return result;
        }

        public static List<AlgoCommentData> ToModel(this List<AlgoCommentEntity> entities)
        {
            var result = new List<AlgoCommentData>();

            foreach (var entity in entities)
            {
                var data = new AlgoCommentData();
                data.AlgoId = entity.PartitionKey;
                data.CommentId = entity.RowKey;
                data.Author = entity.AutorId;
                data.Content = entity.Content;
                data.CreatedOn = entity.CreatedOn;
                data.EditedOn = entity.EditedOn;

                result.Add(data);
            }            

            return result;
        }

        public static AlgoCommentEntity ToEntity(this AlgoCommentData data)
        {
            var result = new AlgoCommentEntity();
            result.PartitionKey = data.AlgoId;
            result.RowKey = data.CommentId;
            result.AutorId = data.Author;
            result.Content = data.Content;
            result.CreatedOn = data.CreatedOn;
            result.EditedOn = data.EditedOn;

            return result;
        }
    }
}
