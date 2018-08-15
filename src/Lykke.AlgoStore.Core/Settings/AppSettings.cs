using Lykke.AlgoStore.Core.Settings.ClientSettings;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.Core.Settings.SlackNotifications;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.AlgoTrades.Client;
using Lykke.AlgoStore.Service.History.Client;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.AlgoStore.Service.Statistics.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.PersonalData.Settings;

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
        public CandlesHistoryServiceClient AlgoStoreCandlesHistoryServiceClient { get; set; }
        public SecurityServiceClientSettings AlgoStoreSecurityServiceClient { get; set; }
        public LoggingServiceClientSettings AlgoStoreLoggingServiceClient { get; set; }
        public AlgoStoreStoppingClientSettings AlgoStoreStoppingClient { get; set; }
        public StatisticsServiceClientSettings AlgoStoreStatisticsClient { get; set; }
        public HistoryServiceClientSettings AlgoStoreHistoryServiceClient { get; set; }
    }
}
