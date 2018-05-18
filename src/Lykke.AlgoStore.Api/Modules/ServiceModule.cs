using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Settings;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.Service.AlgoTrades.Client;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.PersonalData.Contract;
using Lykke.Service.RateCalculator.Client;
using Lykke.Service.Session;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;
using System;
using Lykke.Service.CandlesHistory.Client;

namespace Lykke.AlgoStore.Api.Modules
{
    public class ServiceModule : Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AppSettings> settings, ILog log)
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
            builder.RegisterLykkeServiceClient(_settings.CurrentValue.ClientAccountServiceClient.ServiceUrl);

            builder.RegisterType<ClientSessionsClient>()
                .As<IClientSessionsClient>()
                .WithParameter("serviceUrl", _settings.CurrentValue.AlgoApi.Services.SessionServiceUrl);

            builder.RegisterType<KubernetesApiClient>()
                .As<IKubernetesApiClient>()
                .As<IKubernetesApiReadOnlyClient>()
                .WithParameter("baseUri", new Uri(_settings.CurrentValue.AlgoApi.Kubernetes.Url))
                .WithParameter("credentials", new TokenCredentials(_settings.CurrentValue.AlgoApi.Kubernetes.BasicAuthenticationValue))
                .WithParameter("certificateHash", _settings.CurrentValue.AlgoApi.Kubernetes.CertificateHash)
                .SingleInstance();

            builder.RegisterType<TeamCityClient.TeamCityClient>()
                .As<ITeamCityClient>()
                .WithParameter("settings", _settings.CurrentValue.AlgoApi.TeamCity)
                .SingleInstance();

            builder.RegisterType<AssetsService>()
                .As<IAssetsService>()
                .WithProperty("BaseUri", new System.Uri(_settings.CurrentValue.AlgoApi.Services.AssetServiceUrl));

            builder.RegisterBalancesClient(_settings.CurrentValue.BalancesServiceClient.ServiceUrl, _log);
            builder.RegisterRateCalculatorClient(_settings.CurrentValue.RateCalculatorServiceClient.ServiceUrl, _log);

            builder.RegisterAlgoTradesClient(_settings.CurrentValue.AlgoTradesServiceClient, _log);

            builder.RegisterInstance(new PersonalDataService(_settings.CurrentValue.PersonalDataServiceClient, null))
             .As<IPersonalDataService>()
             .SingleInstance();

            builder.RegisterType<Candleshistoryservice>()
                .As<ICandleshistoryservice>()
                .WithParameter(TypedParameter.From(new Uri(_settings.CurrentValue.CandlesHistoryServiceClient.ServiceUrl)));
        }

        private void RegisterLocalServices(ContainerBuilder builder)
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

            builder.RegisterType<AlgoStoreCommentsService>()
                .As<IAlgoStoreCommentsService>()
                .SingleInstance();

            builder.RegisterType<WalletBalanceService>()
                .As<IWalletBalanceService>()
                .SingleInstance();

            builder.RegisterType<AlgoStoreTradesService>()
                .As<IAlgoStoreTradesService>()
                .WithParameter("maxNumberOfRowsToFetch", _settings.CurrentValue.AlgoApi.MaxNumberOfRowsToFetch)
                .SingleInstance();

            builder.RegisterType<AlgoStoreStatisticsService>()
                .As<IAlgoStoreStatisticsService>()
                .SingleInstance();

            builder.RegisterType<UserRolesService>()
               .As<IUserRolesService>()
               .SingleInstance();

            builder.RegisterType<UserPermissionsService>()
                .As<IUserPermissionsService>()
                .SingleInstance();
        }
    }
}
