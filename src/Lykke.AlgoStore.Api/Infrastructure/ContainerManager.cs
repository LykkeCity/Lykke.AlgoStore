using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.Modules;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Infrastructure
{
    internal static class ContainerManager
    {
        public static IContainer RegisterAlgoApiModules(IServiceCollection services, IReloadingManager<AlgoApiSettings> settings, ILog log)
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterModule(new AlgoApiModule(settings, log));
            builder.RegisterModule(new AspNetCoreModule());

            builder.Populate(services);

            return builder.Build();
        }
    }
}
