using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoRuntimeEntity : TableEntity
    {
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public int AssetAccuracy { get; set; }
        public int AssetInvertedAccuracy { get; set; }
        public string AssetBaseAssetId { get; set; }
        public string AssetQuotingAssetId { get; set; }

        public string TradingAmountAssetId { get; set; }
        public double TradingAmountAmount { get; set; }

        public string AlgoId { get; set; }
        public string TemplateId { get; set; }
    }
}
