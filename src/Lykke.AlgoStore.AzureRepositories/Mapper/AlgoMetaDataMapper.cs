﻿using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoMetaDataMapper
    {
        public static List<AlgoMetaDataEntity> ToEntity(this AlgoClientMetaData data, string partitionKey)
        {
            var result = new List<AlgoMetaDataEntity>();

            if ((data == null) || data.AlgoMetaData.IsNullOrEmptyCollection())
                return result;

            var clientId = data.ClientId;

            foreach (AlgoMetaData algoData in data.AlgoMetaData)
            {
                var res = new AlgoMetaDataEntity();

                res.PartitionKey = partitionKey;
                res.RowKey = algoData.ClientAlgoId;
                res.ClientId = clientId;
                res.Description = algoData.Description;
                res.Name = algoData.Name;
                res.TemplateId = algoData.TemplateId;
                res.ETag = "*";

                result.Add(res);
            }

            return result;
        }
        public static AlgoClientMetaData ToModel(this ICollection<AlgoMetaDataEntity> entities)
        {
            var result = new AlgoClientMetaData { AlgoMetaData = new List<AlgoMetaData>() };

            if ((entities == null) || entities.IsNullOrEmptyCollection())
                return result;

            foreach (var algoEntity in entities)
            {
                if (algoEntity == null)
                    continue;

                result.ClientId = algoEntity.ClientId;
                result.AlgoMetaData.Add(algoEntity.ToAlgoMetaData());
            }

            return result;
        }

        private static AlgoMetaData ToAlgoMetaData(this AlgoMetaDataEntity entity)
        {
            var result = new AlgoMetaData();

            result.ClientAlgoId = entity.RowKey;
            result.Description = entity.Description;
            result.Name = entity.Name;
            result.TemplateId = entity.TemplateId;
            result.Date = entity.Timestamp.DateTime.ToString("yyyy-dd-MM HH:mm:ss");

            return result;
        }
    }
}
