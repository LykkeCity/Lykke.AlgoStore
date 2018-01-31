using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    internal class BuildTypeResponse : BaseInfo
    {
        [JsonProperty(PropertyName = "buildType")]
        public List<BuildType> BuildTypes { get; set; }
    }
}
