using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoClientInstanceMapper
    {
        public static AlgoClientInstanceData ToModel(this AlgoClientInstanceEntity entitiy)
        {
            var result = new AlgoClientInstanceData();

            if (entitiy == null)
                return result;

            var pair = KeyGenerator.ParseKey(entitiy.PartitionKey);
            if (pair == null)
                return result;

            result.ClientId = pair.ClientId;
            result.AlgoId = pair.AlgoId;
            result.InstanceId = entitiy.RowKey;
            result.AssetPair = entitiy.AssetPair;
            result.HftApiKey = entitiy.HftApiKey;
            result.Margin = entitiy.Margin;
            result.TradedAsset = entitiy.TradedAsset;
            result.Volume = entitiy.Volume;

            return result;
        }
        public static AlgoClientInstanceEntity ToEntity(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result.PartitionKey = KeyGenerator.GenerateKey(data.ClientId, data.AlgoId);
            result.RowKey = data.InstanceId;
            result.AssetPair = data.AssetPair;
            result.HftApiKey = data.HftApiKey;
            result.Margin = data.Margin;
            result.TradedAsset = data.TradedAsset;
            result.Volume = data.Volume;
            result.ETag = "*";

            return result;
        }
    }
}
