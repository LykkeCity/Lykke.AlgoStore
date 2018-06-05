using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Api.Models
{
    public class DeleteAlgoModel
    {
        [Required]
        public string AlgoId { get; set; }
        [Required]
        public string AlgoClientId { get; set; }

        public bool ForceDelete { get; set; }
    }
}
