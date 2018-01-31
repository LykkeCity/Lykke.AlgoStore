namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoRatingMetaData : AlgoMetaData
    {
        public string Author { get; set; }
        public double Rating { get; set; }
        public int UsersCount { get; set; }
    }
}
