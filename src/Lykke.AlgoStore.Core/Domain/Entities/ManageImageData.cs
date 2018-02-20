using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class ManageImageData : BaseValidatableData
    {
        [Required]
        public string AlgoId { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string InstanceId { get; set; }
    }
}
