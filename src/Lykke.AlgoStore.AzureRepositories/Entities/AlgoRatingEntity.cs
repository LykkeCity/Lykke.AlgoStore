using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoRatingEntity: TableEntity
    {
        public double Rating { get; set; }
    }
}
