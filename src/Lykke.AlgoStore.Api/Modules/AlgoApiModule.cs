using Autofac;
using Common.Log;
using Lykke.AlgoStore.Core.Identity;
using Lykke.AlgoStore.Core.Settings;
using Lykke.AlgoStore.Services.Identity;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.Api.Modules
{
    public class AlgoApiModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;

        public AlgoApiModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<LykkePrincipal>().As<ILykkePrincipal>().InstancePerLifetimeScope();
        }
    }
}
