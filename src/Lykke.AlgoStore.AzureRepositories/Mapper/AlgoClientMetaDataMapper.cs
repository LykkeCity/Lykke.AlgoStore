using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    internal static class AlgoClientMetaDataMapper
    {
        public static List<AlgoClientMetaDataEntity> ToEntity(this AlgoClientMetaData metadata)
        {
            var result = new List<AlgoClientMetaDataEntity>();

            if (string.IsNullOrWhiteSpace(metadata.ClientId) || metadata.AlgosData.IsNullOrEmptyCollection())
                return result;

            var clientId = metadata.ClientId;

            foreach (AlgoMetaData algoData in metadata.AlgosData)
            {
                var res = new AlgoClientMetaDataEntity();

                res.PartitionKey = clientId;
                res.RowKey = algoData.Id;
                res.Description = algoData.Description;
                res.Name = algoData.Name;

                result.Add(res);
            }

            return result;
        }
        public static AlgoClientMetaData ToModel(this IEnumerable<AlgoClientMetaDataEntity> entities)
        {
            var result = new AlgoClientMetaData { AlgosData = new List<AlgoMetaData>() };

            if (entities.IsNullOrEmptyEnumerable())
                return result;

            var enumerator = entities.GetEnumerator();
            enumerator.Reset();
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                result.ClientId = current.PartitionKey;
                result.AlgosData.Add(current.ToAlgoMetaData());
            }
            else
                return result;

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                result.AlgosData.Add(current.ToAlgoMetaData());
            }

            return result;
        }

        private static AlgoMetaData ToAlgoMetaData(this AlgoClientMetaDataEntity entity)
        {
            return new AlgoMetaData
            {
                Id = entity.RowKey,
                Description = entity.Description,
                Name = entity.Name
            };
        }
    }
}
