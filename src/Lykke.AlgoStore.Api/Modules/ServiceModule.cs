using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Settings;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.Services;
using Lykke.Service.Assets.Client;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.PersonalData.Contract;
using Lykke.Service.Session;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Api.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;

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
                .WithParameter("serviceUrl", _settings.CurrentValue.AlgoApi.Services.SessionServiceUrl);

            builder.RegisterType<DeploymentApiClient.DeploymentApiClient>()
                .As<IDeploymentApiClient>()
                .As<IDeploymentApiReadOnlyClient>()
                .WithProperty("BaseUri", new System.Uri(_settings.CurrentValue.AlgoApi.Services.DeploymentApiServiceUrl))
                .SingleInstance();

            builder.RegisterType<AssetsService>()
                .As<IAssetsService>()
                .WithProperty("BaseUri", new System.Uri(_settings.CurrentValue.AlgoApi.Services.AssetServiceUrl));
            
            builder.RegisterInstance(new PersonalDataService(_settings.CurrentValue.PersonalDataServiceClient, null))
             .As<IPersonalDataService>()
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
