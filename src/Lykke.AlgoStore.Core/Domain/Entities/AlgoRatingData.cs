namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoRatingData
    {
        public double Rating { get; set; }
        public int RatedUsersCount { get; set; }
        public string AlgoId { get; set; }
        public string ClientId { get; set; }
    }
}
