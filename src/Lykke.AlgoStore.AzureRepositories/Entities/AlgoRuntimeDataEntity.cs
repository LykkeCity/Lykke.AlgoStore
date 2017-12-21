using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoRuntimeDataEntity : TableEntity
    {
        public int BuildImageId { get; set; }
        public long ImageId { get; set; }
    }
}
