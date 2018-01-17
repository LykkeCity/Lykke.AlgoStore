using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.Core.Settings.SlackNotifications;

namespace Lykke.AlgoStore.Core.Settings
{
    public class AppSettings
    {
        public AlgoApiSettings AlgoApi { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
