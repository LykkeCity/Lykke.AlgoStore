using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoRuntimeDataMapper
    {
        public static AlgoClientRuntimeData ToModel(this AlgoRuntimeDataEntity entitiy)
        {
            var result = new AlgoClientRuntimeData();

            if (entitiy == null)
                return result;

            result.ClientId = entitiy.PartitionKey;
            result.AlgoId = entitiy.RowKey;
            result.BuildImageId = entitiy.BuildImageId;
            result.ImageId = entitiy.ImageId;

            return result;
        }
        public static AlgoRuntimeDataEntity ToEntity(this AlgoClientRuntimeData data)
        {
            var result = new AlgoRuntimeDataEntity();

            if (data == null)
                return result;

            result.PartitionKey = data.ClientId;
            result.RowKey = data.AlgoId;
            result.BuildImageId = data.BuildImageId;
            result.ImageId = data.ImageId;
            result.ETag = "*";

            return result;
        }
    }
}
