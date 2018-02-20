using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class TailLogData : ManageImageData
    {
        [Required]
        public int Tail { get; set; }
    }
}
