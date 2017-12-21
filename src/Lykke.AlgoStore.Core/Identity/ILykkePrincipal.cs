using System.Security.Claims;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Identity
{
    public interface ILykkePrincipal
    {
        Task<ClaimsPrincipal> GetCurrent();
        string GetToken();
        void InvalidateCache(string token);
    }
}
