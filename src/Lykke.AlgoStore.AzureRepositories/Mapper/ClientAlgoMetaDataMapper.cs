using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    internal static class ClientAlgoMetaDataMapper
    {
        public static List<ClientAlgoMetaDataEntity> ToEntity(this ClientAlgoMetaData metadata)
        {
            var result = new List<ClientAlgoMetaDataEntity>();

            if (string.IsNullOrWhiteSpace(metadata.ClientId) || metadata.AlgosData.IsNullOrEmptyCollection())
                return result;

            var clientId = metadata.ClientId;

            foreach (AlgoMetaData algoData in metadata.AlgosData)
            {
                result.Add(new ClientAlgoMetaDataEntity
                {
                    PartitionKey = clientId,
                    RowKey = algoData.Id,
                    AlgoDataId = algoData.AlgoDataId,
                    Description = algoData.Description,
                    Name = algoData.Name
                });
            }

            return result;
        }
        public static ClientAlgoMetaData ToModel(this IEnumerable<ClientAlgoMetaDataEntity> entities)
        {
            var result = new ClientAlgoMetaData { AlgosData = new List<AlgoMetaData>()};

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

        private static AlgoMetaData ToAlgoMetaData(this ClientAlgoMetaDataEntity entity)
        {
            return new AlgoMetaData
            {
                Id = entity.RowKey,
                AlgoDataId = entity.AlgoDataId,
                Description = entity.Description,
                Name = entity.Name
            };
        }
    }
}
