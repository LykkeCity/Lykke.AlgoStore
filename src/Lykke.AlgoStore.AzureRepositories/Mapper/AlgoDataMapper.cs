using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoDataMapper
    {
        public static AlgoDataEntity ToEntity(this AlgoData algoData)
        {
            var result = new AlgoDataEntity();
            result.PartitionKey = algoData.ClientId;
            result.RowKey = algoData.Id;
            result.Source = algoData.Source;

            return result;
        }

        public static AlgoData ToModel(this AlgoDataEntity entity)
        {
            var result = new AlgoData();
            result.ClientId = entity.PartitionKey;
            result.Id = entity.RowKey;
            result.Source = entity.Source;

            return result;
        }
    }
}
