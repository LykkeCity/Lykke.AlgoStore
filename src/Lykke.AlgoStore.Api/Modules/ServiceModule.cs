using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Common.Log;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Settings;
using Lykke.AlgoStore.Service.AlgoTrades.Client;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.PersonalData.Contract;
using Lykke.Service.RateCalculator.Client;
using Lykke.Service.Session;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Dynamic;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources;
using Lykke.AlgoStore.Api.RealTimeStreaming.Sources.RabbitMq;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.History.Client;
using Lykke.AlgoStore.Service.Statistics.Client;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeAzureTable;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.RabbitMqBroker.Subscriber;
using Newtonsoft.Json;

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

            _services.RegisterAssetsClient(AssetServiceSettings.Create(
                    new Uri(_settings.CurrentValue.AssetsServiceClient.ServiceUrl),
                    _settings.CurrentValue.AlgoApi.Dictionaries.CacheExpirationPeriod),
                _log,
                autoRefresh: true);

            builder.RegisterInstance(_settings.CurrentValue.AlgoApi.RateLimitSettings)
                .AsSelf()
                .SingleInstance();

            RegisterRealTimeDataStreamServices(builder);

            builder.Populate(_services);
        }

        private void RegisterRealTimeDataStreamServices(ContainerBuilder builder)
        {
            var logFactory = LogFactory.Create().AddAzureTable(_settings.Nested(x => x.AlgoApi.Db.LogsConnectionString), AlgoStoreConstants.LogTableName).AddConsole();

            builder.RegisterInstance(logFactory).As<ILogFactory>();


            RabbitMqSubscriptionSettings rabbitMqDummyDataOrderBooks = new RabbitMqSubscriptionSettings()
            {
                ConnectionString = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Dummy.ConnectionString,
                ExchangeName = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Dummy.ExchangeName,
                QueueName = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Dummy.QueueName
            };

            RabbitMqSubscriptionSettings rabbitMqCandles = new RabbitMqSubscriptionSettings()
            {
                ConnectionString = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Candles.ConnectionString,
                ExchangeName = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Candles.ExchangeName,
                QueueName = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Candles.QueueName
            };

            RabbitMqSubscriptionSettings rabbitMqTrades = new RabbitMqSubscriptionSettings()
            {
                ConnectionString = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Trades.ConnectionString,
                ExchangeName = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Trades.ExchangeName,
                QueueName = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Trades.QueueName
            };

            RabbitMqSubscriptionSettings rabbitMqFunctions = new RabbitMqSubscriptionSettings()
            {
                ConnectionString = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Functions.ConnectionString,
                ExchangeName = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Functions.ExchangeName,
                QueueName = _settings.CurrentValue.AlgoApi.RealTimeDataStreaming.RabbitMqSources.Functions.QueueName
            };

            RegisterObservableRabbitMqConnection<OrderBook>(builder, rabbitMqDummyDataOrderBooks, logFactory);
            RegisterObservableRabbitMqConnection<CandleChartingUpdate>(builder, rabbitMqCandles, logFactory);
            RegisterObservableRabbitMqConnection<TradeChartingUpdate>(builder, rabbitMqTrades, logFactory);
            RegisterObservableRabbitMqConnection<FunctionChartingUpdate>(builder, rabbitMqFunctions, logFactory);

            builder.RegisterGeneric(typeof(WebSocketMiddleware<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(WebSocketHandlerBase<>)).InstancePerDependency();
            builder.RegisterType<DummyWebSocketHandler>().InstancePerDependency();
            builder.RegisterType<CandlesWebSocketHandler>().InstancePerDependency();
            builder.RegisterType<WebSocketHandlerBase<TradeChartingUpdate>>().InstancePerDependency();
            builder.RegisterType<WebSocketHandlerBase<FunctionChartingUpdate>>().InstancePerDependency();

        }

        private void RegisterObservableRabbitMqConnection<T>(ContainerBuilder container, RabbitMqSubscriptionSettings exchangeConfiguration, ILogFactory logFactory, string regKey = "") where T : IChartingUpdate
        {
            container.RegisterType<ObservableRabbitMqConnection<T>>()
                .WithParameter("rabbitSettings", exchangeConfiguration)
                .WithParameter("logFactory", logFactory)
                .InstancePerDependency()
                .As<RealTimeDataSourceBase<T>>();
        }

        private void RegisterExternalServices(ContainerBuilder builder)
        {
            var date = DateTime.Now.ToIsoDateTime();

            builder.RegisterLykkeServiceClient(_settings.CurrentValue.ClientAccountServiceClient.ServiceUrl);

            builder.RegisterType<ClientSessionsClient>()
                .As<IClientSessionsClient>()
                .WithParameter("serviceUrl", _settings.CurrentValue.AlgoApi.Services.SessionServiceUrl);

            builder.RegisterType<TeamCityClient.TeamCityClient>()
                .As<ITeamCityClient>()
                .WithParameter("settings", _settings.CurrentValue.AlgoApi.TeamCity)
                .SingleInstance();

            builder.RegisterType<CodeBuildService>()
                .As<ICodeBuildService>()
                .WithParameter("algoNamespaceValue", _settings.CurrentValue.AlgoApi.AlgoNamespaceValue)
                .SingleInstance();

            builder.RegisterBalancesClient(_settings.CurrentValue.BalancesServiceClient.ServiceUrl, _log);
            builder.RegisterRateCalculatorClient(_settings.CurrentValue.RateCalculatorServiceClient.ServiceUrl, _log);

            builder.RegisterAlgoTradesClient(_settings.CurrentValue.AlgoTradesServiceClient, _log);

            builder.RegisterHistoryClient(_settings.CurrentValue.AlgoStoreHistoryServiceClient);

            builder.RegisterInstance(new PersonalDataService(_settings.CurrentValue.PersonalDataServiceClient, null))
             .As<IPersonalDataService>()
             .SingleInstance();

            builder.RegisterAlgoInstanceStoppingClient(_settings.CurrentValue.AlgoStoreStoppingClient.ServiceUrl, _log);

            builder.RegisterType<Candleshistoryservice>()
                .As<ICandleshistoryservice>()
                .WithParameter(TypedParameter.From(new Uri(_settings.CurrentValue.CandlesHistoryServiceClient.ServiceUrl)));

            builder.RegisterType<SecurityClient>()
                .WithParameter("serviceUrl", _settings.CurrentValue.AlgoStoreSecurityServiceClient.ServiceUrl)
                .As<ISecurityClient>()
                .SingleInstance();

            builder.RegisterType<LoggingClient>()
                .WithParameter("serviceUrl", _settings.CurrentValue.AlgoStoreLoggingServiceClient.ServiceUrl)
                .As<ILoggingClient>()
                .SingleInstance();
        }

        private void RegisterLocalServices(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<AlgosService>()
                .As<IAlgosService>()
                .SingleInstance();

            builder.RegisterType<AlgoInstancesService>()
                .As<IAlgoInstancesService>()
                .SingleInstance();

            builder.RegisterType<AlgoStoreService>()
                .As<IAlgoStoreService>()
                .SingleInstance();

            builder.RegisterType<AlgoStoreCommentsService>()
                .As<IAlgoStoreCommentsService>()
                .SingleInstance();

            builder.RegisterType<AlgoStoreClientsService>()
                .As<IAlgoStoreClientsService>()
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
                .WithParameter("statisticsServiceUrl", _settings.CurrentValue.AlgoStoreStatisticsClient.ServiceUrl)
                .SingleInstance();

            builder.RegisterType<AlgoInstanceHistoryService>()
                .As<IAlgoInstanceHistoryService>().SingleInstance();
        }
    }
}
