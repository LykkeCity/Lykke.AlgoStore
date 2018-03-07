using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
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
