using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class BaseAlgoInstance : BaseAlgoData
    {
        [Required]
        public string InstanceId { get; set; }
    }
}
