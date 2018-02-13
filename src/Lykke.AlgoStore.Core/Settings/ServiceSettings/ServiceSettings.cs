using Lykke.Service.PersonalData.Settings;

namespace Lykke.AlgoStore.Core.Settings.ServiceSettings
{
    public class ServiceSettings
    {
        public string SessionServiceUrl { get; set; }
        public string DeploymentApiServiceUrl { get; set; }
        public string AssetServiceUrl { get; set; }
        public string ClientAccountServiceUrl { get; set; }
        public PersonalDataServiceClientSettings PersonalDataServiceClient { get; set; }
    }
}
