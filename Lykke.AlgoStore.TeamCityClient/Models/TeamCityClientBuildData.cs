namespace Lykke.AlgoStore.TeamCityClient.Models
{
    public class TeamCityClientBuildData
    {
        public string BlobDateHeader { get; set; }
        public string BlobVersionHeader { get; set; }
        public string BlobAuthorizationHeader { get; set; }
        public string BlobUrl { get; set; }
        public string AlgoId { get; set; }
    }
}
