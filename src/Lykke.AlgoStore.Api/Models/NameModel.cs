using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Api.Models
{
    public class NameModel : BaseValidatableData
    {
        [JsonProperty("name"), Required]
        public string Name { get; set; }
    }
}
