namespace Lykke.AlgoStore.TeamCityClient.Models
{
    internal class ProblemInfo : BaseInfo
    {
        private const string ToStringPattern = "Problem id: {0}; identity: {1}; type: {2}; details: {3}";

        public string Id { get; set; }
        public string Type { get; set; }
        public string Identity { get; set; }
        public string Details { get; set; }

        public override string ToString()
        {
            return string.Format(ToStringPattern, Id, Identity, Type, Details);
        }
    }
}
