using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    public class ParametersResponse : BaseInfo
    {
        [JsonProperty(PropertyName = "property")]
        public List<Property> Properies { get; set; }
    }
}
