namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoRuntimeData
    {
        public string ImageId { get; set; }
        public string Version { get; set; }
        public TradingAssetData Asset { get; set; }
        public TradingAmountData TradingAmount { get; set; }
        public long GetImageIdAsNumber()
        {
            if (long.TryParse(ImageId, out var imageId))
                return imageId;
            return 0;
        }
    }
}
