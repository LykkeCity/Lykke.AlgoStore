using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Infrastructure.Authentication
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddLykkeAuthentication(this IServiceCollection services)
        {
            return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddScheme<LykkeAuthOptions, LykkeAuthHandler>("Bearer", options => { });
        }
    }
}
