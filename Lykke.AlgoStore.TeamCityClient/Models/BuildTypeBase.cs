using Newtonsoft.Json;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    [JsonObject(Title = "buildType")]
    internal class BuildTypeBase
    {
        public string Id { get; set; }
    }
}
