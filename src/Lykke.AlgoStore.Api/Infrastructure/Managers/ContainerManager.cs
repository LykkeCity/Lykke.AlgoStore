using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AlgoStore.Api.Modules;
using Lykke.AlgoStore.Core.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Api.Infrastructure.Managers
{
    internal static class ContainerManager
    {
        public static IContainer RegisterAlgoApiModules(IServiceCollection services, IReloadingManager<AppSettings> settings, ILog log)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new AspNetCoreModule());
            builder.RegisterModule(new AlgoApiModule(settings, log));
            builder.RegisterModule(new AlgoRepositoryModule(settings, log));
            builder.RegisterModule(new ServiceModule(settings.Nested(x => x.AlgoApi)));

            builder.Populate(services);

            return builder.Build();
        }
    }
}
