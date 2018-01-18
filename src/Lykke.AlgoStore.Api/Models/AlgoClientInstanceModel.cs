namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoClientInstanceModel
    {
        public string InstanceId { get; set; }
        public string AlgoId { get; set; }
        public string HftApiKey { get; set; }
        public string AssetPair { get; set; }
        public string TradedAsset { get; set; }
        public double Volume { get; set; }
        public double Margin { get; set; }
    }
}
