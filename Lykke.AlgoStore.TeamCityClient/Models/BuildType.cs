namespace Lykke.AlgoStore.TeamCityClient.Models
{
    internal class BuildType : BaseInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public string WebUrl { get; set; }

    }
}
