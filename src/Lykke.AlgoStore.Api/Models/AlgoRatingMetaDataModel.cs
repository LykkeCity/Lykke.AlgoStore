namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoRatingMetaDataModel : AlgoMetaDataModel
    {
        public string ClientId { get; set; }
        public double Rating { get; set; }
        public int UsersCount { get; set; }
    }
}
