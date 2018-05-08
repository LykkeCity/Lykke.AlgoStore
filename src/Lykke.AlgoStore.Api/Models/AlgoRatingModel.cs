namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoRatingModel
    {
        public string AlgoId { get; set; }
        public string ClientId { get; set; }
        public int RatedUsersCount { get; set; }
        public double Rating { get; set; }
    }
}
