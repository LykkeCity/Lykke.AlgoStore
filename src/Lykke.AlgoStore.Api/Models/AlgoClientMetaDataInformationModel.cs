using Lykke.AlgoStore.Core.Enumerators;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoClientMetaDataInformationModel
    {
        public string AlgoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public AlgoVisibility AlgoVisibility { get; set; }
        public string Author { get; set; }

        public double Rating { get; set; }
        public int RatedUsersCount { get; set; }
        public int UsersCount { get; set; }      

        public AlgoMetaDataInformationModel AlgoMetaDataInformation { get; set; }
    }
}
