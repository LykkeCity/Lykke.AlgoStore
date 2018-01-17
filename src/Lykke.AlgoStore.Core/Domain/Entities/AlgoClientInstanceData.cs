using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoClientInstanceData : BaseAlgoInstance
    {
        [Required]
        public string HftApiKey { get; set; }
        [Required]
        public string AssetPair { get; set; }
        [Required]
        public string TradedAsset { get; set; }
        [Required]
        public double Volume { get; set; }
        [Required]
        public double Margin { get; set; }
    }
}
