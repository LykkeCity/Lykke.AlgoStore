using System;

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
    }

    public class DictionariesSettings
    {
        public TimeSpan CacheExpirationPeriod { get; set; }
    }
}
