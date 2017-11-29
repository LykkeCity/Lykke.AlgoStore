using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class DeployImageData : BaseValidatableData
    {
        public string ClientId { get; set; }
        [Required]
        public string ImageId { get; set; }
        public byte[] Data { get; set; }
    }
}
