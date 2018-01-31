using Newtonsoft.Json;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    [JsonObject(Title = "build")]
    internal class BuildRequest
    {
        public bool Personal { get; set; }
        public BuildTypeBase BuildType { get; set; }
        public Properties Properties { get; set; }
    }
}
