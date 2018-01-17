using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoMetaDataEntity : TableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
