using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoClientInstanceEntity : TableEntity
    {
        public string HftApiKey { get; set; }
        public string AssetPair { get; set; }
        public string TradedAsset { get; set; }
        public double Volume { get; set; }
        public double Margin { get; set; }
    }
}
