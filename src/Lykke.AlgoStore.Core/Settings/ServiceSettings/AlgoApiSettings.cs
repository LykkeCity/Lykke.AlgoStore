﻿using System;
using Lykke.AlgoStore.Core.Settings.ServiceSettings.RealTimeData;
using Lykke.AlgoStore.Security.InstanceAuth;

namespace Lykke.AlgoStore.Core.Settings.ServiceSettings
{
    public class AlgoApiSettings
    {
        public DbSettings Db { get; set; }
        public DictionariesSettings Dictionaries { get; set; }
        public ServiceSettings Services { get; set; }
        public TeamCitySettings TeamCity { get; set; }
        public int MaxNumberOfRowsToFetch { get; set; }
        public string AlgoNamespaceValue { get; set; }
        public RealTimeDataSettings RealTimeDataStreaming { get; set; }
        public RateLimitSettings RateLimitSettings { get; set; }
    }

    public class DictionariesSettings
    {
        public TimeSpan CacheExpirationPeriod { get; set; }
    }
}
