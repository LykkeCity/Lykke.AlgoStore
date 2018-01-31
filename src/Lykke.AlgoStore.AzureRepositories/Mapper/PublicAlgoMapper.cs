using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class PublicAlgoMapper
    {
        public static readonly string PartitionKey = "PublicAlgos";

        public static PublicAlgoData ToModel(this PublicAlgoEntity entitiy)
        {
            var result = new PublicAlgoData();

            if (entitiy == null)
                return result;

            var pair = KeyGenerator.ParseKey(entitiy.RowKey);
            if (pair == null)
                return result;

            result.ClientId = pair.ClientId;
            result.AlgoId = pair.AlgoId;


            return result;
        }
        public static PublicAlgoEntity ToEntity(this PublicAlgoData data)
        {
            var result = new PublicAlgoEntity();

            if (data == null)
                return result;

            result.PartitionKey = PartitionKey;
            result.RowKey = KeyGenerator.GenerateKey(data.ClientId, data.AlgoId);
            result.ETag = "*";

            return result;
        }
    }
}
