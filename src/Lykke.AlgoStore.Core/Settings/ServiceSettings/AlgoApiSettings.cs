﻿using Lykke.AlgoStore.Core.Settings.SlackNotifications;

namespace Lykke.AlgoStore.Core.Settings.ServiceSettings
{
    public class AlgoApiSettings
    {
        public DbSettings Db { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}