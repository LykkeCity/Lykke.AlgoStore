using System.Collections.Generic;

namespace Lykke.AlgoStore.Api.Models
{
    public class ErrorModel
    {
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, List<string>> ModelErrors { get; set; }
    }
}
