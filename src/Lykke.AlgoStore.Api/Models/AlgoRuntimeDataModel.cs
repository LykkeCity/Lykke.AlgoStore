namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoRuntimeDataModel
    {
        public string ImageId { get; set; }
        public string Version { get; set; }
        public TradingAssetDataModel Asset { get; set; }
        public TradingAmountDataModel TradingAmount { get; set; }
    }
}
