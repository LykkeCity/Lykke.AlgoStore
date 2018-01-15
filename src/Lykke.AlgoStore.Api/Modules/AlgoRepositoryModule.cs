using Autofac;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.AzureRepositories.Utils;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Settings;
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
            builder.RegisterInstance(AzureTableStorage<AlgoMetaDataEntity>.Create(reloadingDbManager, AlgoMetaDataRepository.TableName, _log));
            builder.RegisterInstance(AzureTableStorage<AlgoRuntimeDataEntity>.Create(reloadingDbManager, AlgoRuntimeDataRepository.TableName, _log));

            builder.RegisterInstance<IStorageConnectionManager>(new StorageConnectionManager(reloadingDbManager));

            builder.RegisterType<AlgoBlobRepository>().As<IAlgoBlobReadOnlyRepository>().As<IAlgoBlobRepository>();
            builder.RegisterType<AlgoMetaDataRepository>().As<IAlgoMetaDataReadOnlyRepository>().As<IAlgoMetaDataRepository>();
            builder.RegisterType<AlgoRuntimeDataRepository>().As<IAlgoRuntimeDataReadOnlyRepository>().As<IAlgoRuntimeDataRepository>();
        }
    }
}
