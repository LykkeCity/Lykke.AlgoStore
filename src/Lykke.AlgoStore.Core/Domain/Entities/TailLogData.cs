using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class TailLogData : BaseValidatableData
    {
        [Required]
        public string AlgoId { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public int Tail { get; set; }
    }
}
