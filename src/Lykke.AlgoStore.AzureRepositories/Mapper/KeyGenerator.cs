using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class KeyGenerator
    {
        private const string PartitionKeySeparator = "_";
        private const string PartitionKeyPattern = "{0}{1}{2}";

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
