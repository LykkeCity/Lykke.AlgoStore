using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.AlgoMetaDataModels;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoMetaDataMapper
    {
        public static List<AlgoMetaDataEntity> ToEntity(this AlgoClientMetaData data)
        {
            var result = new List<AlgoMetaDataEntity>();

            if ((data == null) || data.AlgoMetaData.IsNullOrEmptyCollection())
                return result;

            foreach (AlgoMetaData algoData in data.AlgoMetaData)
            {
                var res = new AlgoMetaDataEntity();

                res.PartitionKey = data.ClientId;
                res.RowKey = algoData.AlgoId;
                res.Description = algoData.Description;
                res.Name = algoData.Name;
                res.Author = data.Author;
                res.ETag = "*";
                res.AlgoMetaDataInformationJSON = algoData.AlgoMetaDataInformationJSON;

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

                result.ClientId = algoEntity.PartitionKey;
                if (!string.IsNullOrWhiteSpace(algoEntity.Author))
                    result.Author = algoEntity.Author;
                result.AlgoMetaData.Add(algoEntity.ToAlgoMetaData());
            }

            return result;
        }

        private static AlgoMetaData ToAlgoMetaData(this AlgoMetaDataEntity entity)
        {
            var result = new AlgoMetaData();

            result.AlgoId = entity.RowKey;
            result.Description = entity.Description;
            result.Name = entity.Name;
            result.Date = entity.Timestamp.DateTime.ToString("yyyy-dd-MM HH:mm:ss");
            result.AlgoMetaDataInformationJSON = entity.AlgoMetaDataInformationJSON;

            return result;
        }

        public static AlgoClientMetaDataInformation ToAlgoMetaInformation(this AlgoMetaDataEntity entity)
        {
            var result = new AlgoClientMetaDataInformation();

            result.AlgoId = entity.RowKey;
            result.Description = entity.Description;
            result.Name = entity.Name;
            result.Date = entity.Timestamp.DateTime.ToString("yyyy-dd-MM HH:mm:ss");

            if (!string.IsNullOrEmpty(entity.AlgoMetaDataInformationJSON))
            {
                result.AlgoMetaDataInformation = JsonConvert.DeserializeObject<AlgoMetaDataInformation>(entity.AlgoMetaDataInformationJSON);
            }

            return result;
        }
    }
}
