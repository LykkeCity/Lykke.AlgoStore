using System;
using System.Security.Claims;

namespace Lykke.AlgoStore.Services.Identity
{
    internal class PrincipalCashItem
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public DateTime LastRefresh { get; private set; }

        public static PrincipalCashItem Create(ClaimsPrincipal src)
        {
            return new PrincipalCashItem
            {
                LastRefresh = DateTime.UtcNow,
                ClaimsPrincipal = src
            };
        }
    }
}
