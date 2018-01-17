using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoClientInstanceMapper
    {
        private const string PartitionKeySeparator = "_";
        private const string PartitionKeyPattern = "{0}{1}{2}";

        public static AlgoClientInstanceData ToModel(this AlgoClientInstanceEntity entitiy)
        {
            var result = new AlgoClientInstanceData();

            if (entitiy == null)
                return result;

            var pair = ParseKey(entitiy.PartitionKey);
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

            result.PartitionKey = GenerateKey(data.ClientId, data.AlgoId);
            result.RowKey = data.InstanceId;
            result.AssetPair = data.AssetPair;
            result.HftApiKey = data.HftApiKey;
            result.Margin = data.Margin;
            result.TradedAsset = data.TradedAsset;
            result.Volume = data.Volume;
            result.ETag = "*";

            return result;
        }

        public static string GenerateKey(string clientId, string algoId)
        {
            return string.Format(PartitionKeyPattern, clientId, PartitionKeySeparator, algoId);
        }
        public static BaseAlgoData ParseKey(string partitionKey)
        {
            var values = partitionKey.Split(PartitionKeySeparator);
            if (values == null || values.Length != 2)
                return null;

            return new BaseAlgoData
            {
                ClientId = values[0],
                AlgoId = values[1]
            };
        }
    }
}
