using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Api.Models
{
    public class UploadAlgoBinaryModel
    {
        public IFormFile Data { get; set; }
        public string AlgoId { get; set; }
    }
}
