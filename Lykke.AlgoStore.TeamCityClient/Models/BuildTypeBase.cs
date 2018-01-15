using Newtonsoft.Json;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    [JsonObject(Title = "buildType")]
    public class BuildTypeBase
    {
        public string Id { get; set; }
    }
}
