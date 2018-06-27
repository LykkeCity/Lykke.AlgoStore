using Autofac;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.AzureRepositories.Utils;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.Api.Modules
{
    public class AlgoRepositoryModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;

        public AlgoRepositoryModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log).SingleInstance();

            var reloadingDbManager = _settings.ConnectionString(x => x.AlgoApi.Db.TableStorageConnectionString);
            builder.RegisterInstance(AzureBlobStorage.Create(reloadingDbManager));
            builder.RegisterInstance(AzureTableStorage<AlgoEntity>.Create(reloadingDbManager, AlgoRepository.TableName, _log));
            builder.RegisterInstance(AzureTableStorage<AlgoRuntimeDataEntity>.Create(reloadingDbManager, AlgoRuntimeDataRepository.TableName, _log));
            builder.RegisterInstance(AzureTableStorage<PublicAlgoEntity>.Create(reloadingDbManager, PublicAlgosRepository.TableName, _log));
            builder.RegisterInstance(AzureTableStorage<AlgoRatingEntity>.Create(reloadingDbManager, AlgoRatingsRepository.TableName, _log));
            builder.RegisterInstance(AzureTableStorage<AlgoCommentEntity>.Create(reloadingDbManager, AlgoCommentsRepository.TableName, _log));
            builder.RegisterInstance(AzureTableStorage<StatisticsSummaryEntity>.Create(reloadingDbManager, StatisticsRepository.TableName, _log));
            
            builder.RegisterInstance<IStorageConnectionManager>(new StorageConnectionManager(reloadingDbManager));

            builder.RegisterInstance<IAlgoClientInstanceRepository>(
                    AzureRepoFactories.CreateAlgoClientInstanceRepository(reloadingDbManager, _log))
                .SingleInstance();

            builder.RegisterType<AlgoBlobRepository>().As<IAlgoBlobReadOnlyRepository>().As<IAlgoBlobRepository>();
            builder.RegisterType<AlgoRepository>().As<IAlgoReadOnlyRepository>().As<IAlgoRepository>();
            builder.RegisterType<AlgoRuntimeDataRepository>().As<IAlgoRuntimeDataReadOnlyRepository>().As<IAlgoRuntimeDataRepository>();
            

            builder.RegisterType<StatisticsRepository>().As<IStatisticsRepository>();
            
            builder.RegisterType<AlgoRatingsRepository>().As<IAlgoRatingsRepository>();
            builder.RegisterType<PublicAlgosRepository>().As<IPublicAlgosRepository>();
            builder.RegisterType<AlgoCommentsRepository>().As<IAlgoCommentsRepository>();
        }
    }
}
