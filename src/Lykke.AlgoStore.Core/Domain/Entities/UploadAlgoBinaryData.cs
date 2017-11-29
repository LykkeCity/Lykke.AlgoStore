using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
