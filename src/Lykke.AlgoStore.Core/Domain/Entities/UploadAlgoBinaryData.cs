using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class UploadAlgoBinaryData
    {
        public IFormFile Data { get; set; }
        public string AlgoId { get; set; }
    }
}
