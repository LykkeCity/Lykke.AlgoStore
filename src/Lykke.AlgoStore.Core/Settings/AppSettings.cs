using Lykke.AlgoStore.Core.Settings.ClientSettings;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.Core.Settings.SlackNotifications;
using Lykke.Service.PersonalData.Settings;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.Balances.Client;
using Lykke.AlgoStore.Service.AlgoTrades.Client;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.AlgoStore.Service.Security.Client;

namespace Lykke.AlgoStore.Core.Settings
{
    public class AppSettings
    {
        public AlgoApiSettings AlgoApi { get; set; }
        public PersonalDataServiceClientSettings PersonalDataServiceClient { get; set; }
        public AlgoTradesServiceClientSettings AlgoTradesServiceClient { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public ClientAccountServiceClientSettings ClientAccountServiceClient { get; set; }
        public BalancesServiceClientSettings BalancesServiceClient { get; set; }
        public RateCalculatorClientSettings RateCalculatorServiceClient { get; set; }
        public CandlesHistoryServiceClient CandlesHistoryServiceClient { get; set; }
        public SecurityServiceClientSettings AlgoStoreSecurityServiceClient { get; set; }
        public LoggingServiceClientSettings LoggingServiceClient { get; set; }
    }
}
