using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    internal static class AlgoMetaDataMapper
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

                result.Add(res);
            }

            return result;
        }
        public static AlgoClientMetaData ToModel(this IEnumerable<AlgoMetaDataEntity> entities)
        {
            var result = new AlgoClientMetaData { AlgoMetaData = new List<AlgoMetaData>() };

            if ((entities == null) || entities.IsNullOrEmptyEnumerable())
                return result;

            var enumerator = entities.GetEnumerator();
            enumerator.Reset();
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current == null)
                    return result;

                result.ClientId = current.ClientId;
                result.AlgoMetaData.Add(current.ToAlgoMetaData());
            }
            else
                return result;

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current == null)
                    continue;

                result.AlgoMetaData.Add(current.ToAlgoMetaData());
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

            return result;
        }
    }
}
