using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoRuntimeDataEntity : TableEntity
    {
        public int BuildId { get; set; }
        public string PodId { get; set; }
    }
}
