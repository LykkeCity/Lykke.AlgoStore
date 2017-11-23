using Autofac;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.Api.Modules
{
    public class AlgoRepositoryModule : Module
    {
        private readonly IReloadingManager<AlgoApiSettings> _settings;
        private readonly ILog _log;

        public AlgoRepositoryModule(IReloadingManager<AlgoApiSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log).SingleInstance();

            builder.RegisterInstance<IAlgoClientMetaDataRepository>(new AlgoClientMetaDataRepository(_settings.ConnectionString(x => x.Db.TableStorageConnectionString), _log));
        }
    }
}
