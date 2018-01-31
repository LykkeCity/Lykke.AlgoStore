using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    internal class ParametersResponse : BaseInfo
    {
        [JsonProperty(PropertyName = "property")]
        public List<Property> Properies { get; set; }
    }
}
