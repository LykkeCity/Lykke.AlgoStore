using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class UploadAlgoStringData : BaseValidatableData
    {
        [Required]
        public string AlgoId { get; set; }
        [Required]
        public string Data { get; set; }
    }
}
