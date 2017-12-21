using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.Services;
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
            RegisterExternalServices(builder);
            RegisterLocalServices(builder);

            builder.Populate(_services);
        }

        private void RegisterExternalServices(ContainerBuilder builder)
        {

            builder.RegisterType<ClientSessionsClient>()
                .As<IClientSessionsClient>()
                .WithParameter("serviceUrl", _settings.CurrentValue.Services.SessionServiceUrl);

            builder.RegisterType<DeploymentApiClient.DeploymentApiClient>()
                .As<IDeploymentApiClient>()
                .As<IDeploymentApiReadOnlyClient>()
                .WithProperty("BaseUri", new System.Uri(_settings.CurrentValue.Services.DeploymentApiServiceUrl))
                .SingleInstance();
        }

        private static void RegisterLocalServices(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<AlgoStoreClientDataService>()
                .As<IAlgoStoreClientDataService>()
                .SingleInstance();
            builder.RegisterType<AlgoStoreService>()
                .As<IAlgoStoreService>()
                .SingleInstance();
        }
    }
}
