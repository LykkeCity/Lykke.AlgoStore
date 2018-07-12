namespace Lykke.AlgoStore.TeamCityClient.Models
{
    public class TeamCityClientBuildData
    {
        public string BlobDateHeader { get; set; }
        public string BlobVersionHeader { get; set; }
        public string BlobAuthorizationHeader { get; set; }
        public string StorageAccountName { get; set; }
        public string ContainerName { get; set; }
        public string InstanceId { get; set; }
        public string AccessKey { get; set; }
        public string BlobUrl { get; set; }
        public string AlgoId { get; set; }
        public string TradedAsset { get; set; }
        public string AssetPair { get; set; }
        public string HftApiUrl { get; set; }
        public string HftApiKey { get; set; }
        public string WalletApiKey { get; set; }
        public double Margin { get; set; }
        public double Volume { get; set; }
        public string AlgoInstanceParameters { get; set; }
    }
}
