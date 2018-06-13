using System;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    public class Build : BuildBase
    {
        public string Number { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }

        public Agent Agent { get; set; }
        public ProblemOccurrence ProblemOccurrences { get; set; }
        public BaseInfo RelatedIssues { get; set; }
        public BaseInfo Statistics { get; set; }

        public BuildStatuses GetBuildStatus()
        {
            if (Enum.TryParse<BuildStatuses>(Status, true, out var result))
                return result;

            return BuildStatuses.Undefined;
        }
    }
}
