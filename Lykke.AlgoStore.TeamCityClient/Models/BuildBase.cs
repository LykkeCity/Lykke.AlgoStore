using System;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    public class BuildBase : BaseInfo
    {
        public int Id { get; set; }
        public string BuildTypeId { get; set; }
        public string State { get; set; }
        public bool Personal { get; set; }

        public BuildStates GetBuildState()
        {
            if (Enum.TryParse<BuildStates>(State, true, out var result))
                return result;

            return BuildStates.Undefined;
        }
    }
}
