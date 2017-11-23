using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoDataMapper
    {
        public static AlgoDataEntity ToEntity(this AlgoData data, string partitionKey)
        {
            var result = new AlgoDataEntity();
            result.PartitionKey = partitionKey;
            result.RowKey = data.ClientAlgoId;
            result.TemplateId = data.TemplateId;
            result.Source = data.Source;

            return result;
        }

        public static AlgoData ToModel(this AlgoDataEntity entity)
        {
            var result = new AlgoData();
            result.ClientAlgoId = entity.RowKey;
            result.TemplateId = entity.TemplateId;
            result.Source = entity.Source;

            return result;
        }
    }
}
