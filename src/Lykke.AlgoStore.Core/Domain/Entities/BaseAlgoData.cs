using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class BaseAlgoData : BaseValidatableData
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string AlgoId { get; set; }
    }
}
