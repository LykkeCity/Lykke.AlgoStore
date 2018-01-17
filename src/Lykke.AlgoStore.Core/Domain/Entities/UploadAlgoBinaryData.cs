using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class UploadAlgoBinaryData : BaseValidatableData
    {
        [Required]
        public IFormFile Data { get; set; }
        [Required]
        public string AlgoId { get; set; }
    }
}
