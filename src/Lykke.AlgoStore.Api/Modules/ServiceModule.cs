using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.Service.Assets.Client;
using Lykke.Service.Session;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;
using System;

namespace Lykke.AlgoStore.Api.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AlgoApiSettings> _settings;
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AlgoApiSettings> settings)
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
                .WithParameter("serviceUrl", _settings.CurrentValue.Services.SessionServiceUrl);

            builder.RegisterType<KubernetesApiClient>()
                .As<IKubernetesApiClient>()
                .As<IKubernetesApiReadOnlyClient>()
                .WithParameter("baseUri", new Uri(_settings.CurrentValue.Kubernetes.Url))
                .WithParameter("credentials", new TokenCredentials(_settings.CurrentValue.Kubernetes.BasicAuthenticationValue))
                .WithParameter("certificateHash", _settings.CurrentValue.Kubernetes.CertificateHash)
                .SingleInstance();

            builder.RegisterType<AssetsService>()
                .As<IAssetsService>()
                .WithProperty("BaseUri", new System.Uri(_settings.CurrentValue.Services.AssetServiceUrl));

            builder.RegisterInstance<ITeamCityClient>(new TeamCityClient.TeamCityClient(_settings.CurrentValue.TeamCity));
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
