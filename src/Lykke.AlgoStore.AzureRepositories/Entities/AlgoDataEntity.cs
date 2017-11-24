using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoDataEntity : TableEntity
    {
        public string Source { get; set; }
    }
}
