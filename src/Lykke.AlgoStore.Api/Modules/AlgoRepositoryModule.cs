using Autofac;
using AzureStorage.Blob;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Repositories;
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

            builder.RegisterInstance<IAlgoMetaDataRepository>(new AlgoMetaDataRepository(_settings.ConnectionString(x => x.AlgoApi.Db.TableStorageConnectionString), _log));
            builder.RegisterInstance<IAlgoDataRepository>(new AlgoDataRepository(_settings.ConnectionString(x => x.AlgoApi.Db.TableStorageConnectionString), _log));
            builder.RegisterInstance<IAlgoRuntimeDataRepository>(new AlgoRuntimeDataRepository(_settings.ConnectionString(x => x.AlgoApi.Db.TableStorageConnectionString), _log));
            builder.RegisterInstance<IAlgoTemplateDataRepository>(new AlgoTemplateDataRepository(_settings.ConnectionString(x => x.AlgoApi.Db.TableStorageConnectionString), _log));
            builder.RegisterInstance<IAlgoBlobRepository<byte[]>>(new AlgoBlobBinaryRepository(AzureBlobStorage.Create(_settings.ConnectionString(x => x.AlgoApi.Db.TableStorageConnectionString))));
            builder.RegisterInstance<IAlgoBlobRepository<string>>(new AlgoBlobStringRepository(AzureBlobStorage.Create(_settings.ConnectionString(x => x.AlgoApi.Db.TableStorageConnectionString))));

           
        }
    }
}
