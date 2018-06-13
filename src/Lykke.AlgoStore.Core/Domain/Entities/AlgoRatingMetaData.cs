namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoRatingMetaData : AlgoData
    {
        public string Author { get; set; }
        public double Rating { get; set; }
        public int RatedUsersCount { get; set; }
        public int UsersCount { get; set; }
    }
}
