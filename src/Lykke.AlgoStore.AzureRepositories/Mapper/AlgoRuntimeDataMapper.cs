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
            result.BuildId = entitiy.BuildId;
            result.PodId = entitiy.PodId;

            return result;
        }
        public static AlgoRuntimeDataEntity ToEntity(this AlgoClientRuntimeData data)
        {
            var result = new AlgoRuntimeDataEntity();

            if (data == null)
                return result;

            result.PartitionKey = data.ClientId;
            result.RowKey = data.AlgoId;
            result.BuildId = data.BuildId;
            result.PodId = data.PodId;
            result.ETag = "*";

            return result;
        }
    }
}
