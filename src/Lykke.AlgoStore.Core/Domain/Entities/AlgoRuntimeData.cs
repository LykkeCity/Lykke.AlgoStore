namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoRuntimeData
    {
        public string ImageId { get; set; }
        public string TemplateId { get; set; }
        public TradingAssetData Asset { get; set; }
        public TradingAmountData TradingAmount { get; set; }
    }
}
