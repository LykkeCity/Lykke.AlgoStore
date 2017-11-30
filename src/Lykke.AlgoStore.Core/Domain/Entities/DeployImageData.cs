using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class DeployImageData : BaseValidatableData
    {
        [Required]
        public string AlgoId { get; set; }
        [Required]
        public string ClientId { get; set; }
    }
}
