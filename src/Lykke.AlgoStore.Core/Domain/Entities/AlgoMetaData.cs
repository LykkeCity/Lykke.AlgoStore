using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoMetaData : BaseValidatableData
    {
        [Required]
        public string ClientAlgoId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string TemplateId { get; set; }
    }
}
