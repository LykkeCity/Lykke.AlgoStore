using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.Models
{
    public class UploadAlgoBinaryModel
    {
        public IFormFile Data { get; set; }
        public string AlgoId { get; set; }
    }
}
