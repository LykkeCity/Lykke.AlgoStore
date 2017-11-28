using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AlgoStore.Core.Identity;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Services.Identity;
using Lykke.Service.Session;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Api.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AlgoApiSettings> _settings;
        private readonly ILog _log;
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AlgoApiSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterLocalTypes(builder);
            RegisterLocalServices(builder);
            RegisterExternalServices(builder);

            //builder.RegisterType<RequestContext>().As<IRequestContext>().InstancePerLifetimeScope();
            builder.RegisterType<LykkePrincipal>().As<ILykkePrincipal>().InstancePerLifetimeScope();

            builder.Populate(_services);
        }

        private void RegisterExternalServices(ContainerBuilder builder)
        {
            //builder.RegisterType<ClientAccountService>()
            //    .As<IClientAccountService>()
            //    .WithParameter("baseUri", new Uri(_settings.CurrentValue.Services.ClientAccountServiceUrl));

            //builder.RegisterType<ClientAccountClient>()
            //    .As<IClientAccountClient>()
            //    .WithParameter("serviceUrl", _settings.CurrentValue.Services.ClientAccountServiceUrl)
            //    .WithParameter("log", _log)
            //    .SingleInstance();

            builder.RegisterType<ClientSessionsClient>()
                .As<IClientSessionsClient>()
                .WithParameter("serviceUrl", _settings.CurrentValue.Services.SessionServiceUrl);
        }

        private static void RegisterLocalServices(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            //builder.RegisterType<StartupManager>()
            //    .As<IStartupManager>();

            //builder.RegisterType<ShutdownManager>()
            //    .As<IShutdownManager>();
        }

        private void RegisterLocalTypes(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log).As<ILog>().SingleInstance();
            builder.RegisterInstance(_settings.CurrentValue).SingleInstance();
        }
    }
}
