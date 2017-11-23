using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoTemplateDataMapper
    {
        public static List<AlgoTemplateData> ToModel(this IEnumerable<AlgoTemplateDataEntity> entities)
        {
            var result = new List<AlgoTemplateData>();

            return result;
        }

        public static AlgoTemplateData ToModel(this AlgoTemplateDataEntity entity)
        {
            var result = new AlgoTemplateData();

            result.Id = entity.RowKey;
            result.Description = entity.Description;
            result.Source = entity.Source;

            return result;
        }
    }
}
