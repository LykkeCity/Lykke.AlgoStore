namespace Lykke.AlgoStore.TeamCityClient.Models
{
    public class ProblemOccurrence : BaseInfo
    {
        public int Count { get; set; }
        public bool Default { get; set; }
        public int NewFailed { get; set; }
    }
}
