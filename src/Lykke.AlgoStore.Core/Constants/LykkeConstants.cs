using System;

namespace Lykke.AlgoStore.Core.Constants
{
    public static class LykkeConstants
    {
        public static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(3);
        public static readonly TimeSpan SessionRefreshPeriod = TimeSpan.FromDays(1);
    }
}
