using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Api.Infrastructure.Context
{
    public class RequestContext : IRequestContext
    {
        private readonly HttpContext _httpContext;

        public RequestContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public string GetClientId()
        {
            return _httpContext.User?.Identity?.Name;
        }

        public string GetPartnerId()
        {
            var identity = (ClaimsIdentity)_httpContext?.User.Identity;
            return identity?.Claims.FirstOrDefault(x => x.Type == "PartnerId")?.Value;
        }
    }
}
