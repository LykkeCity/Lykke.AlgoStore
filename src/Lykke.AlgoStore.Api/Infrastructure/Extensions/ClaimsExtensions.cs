using System.Linq;
using System.Security.Claims;

namespace Lykke.AlgoStore.Api.Infrastructure.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetClientId(this ClaimsPrincipal user)
        {
            return user?.Identity?.Name;
        }

        public static string GetPartnerId(this ClaimsPrincipal user)
        {
            var identity = (ClaimsIdentity)user.Identity;
            return identity?.Claims.FirstOrDefault(x => x.Type == "PartnerId")?.Value;
        }
    }
}
