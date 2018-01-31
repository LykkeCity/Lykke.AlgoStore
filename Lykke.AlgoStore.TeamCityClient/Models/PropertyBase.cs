using Newtonsoft.Json;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    [JsonObject(Title = "property")]
    internal class PropertyBase
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
