using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoTemplateDataMapper
    {
        public static List<AlgoTemplateData> ToModel(this IEnumerable<AlgoTemplateDataEntity> entities)
        {
            var result = new List<AlgoTemplateData>();

            if (entities.IsNullOrEmptyEnumerable())
                return result;

            foreach (var entity in entities)
            {
                result.Add(entity.ToModel());
            }

            return result;
        }
        public static AlgoTemplateData ToModel(this AlgoTemplateDataEntity entity)
        {
            if (entity == null)
                return null;

            var result = new AlgoTemplateData();

            result.TemplateId = entity.RowKey;
            result.LanguageId = entity.LanguageId;
            result.Description = entity.Description;
            result.Source = entity.Source;
            result.Version = entity.Version;
            result.Branch = entity.Branch;
            result.Build = entity.Build;

            return result;
        }
    }
}
