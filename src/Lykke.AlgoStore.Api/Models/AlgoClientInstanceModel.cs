namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoClientInstanceModel
    {
        public string InstanceId { get; set; }
        public string WalletId { get; set; }
        public string AlgoId { get; set; }
        public string AlgoClientId { get; set; }
        public string InstanceName { get; set; }
        public AlgoMetaDataInformationModel AlgoMetaDataInformation { get; set; }
    }
}
