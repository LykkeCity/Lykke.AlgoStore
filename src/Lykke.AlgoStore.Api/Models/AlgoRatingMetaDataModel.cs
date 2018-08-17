namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoRatingMetaDataModel : AlgoDataModel
    {
        public string Author { get; set; }
        public double Rating { get; set; }
        public int RatedUsersCount { get; set; }
        public int UsesCount { get; set; }
    }
}
