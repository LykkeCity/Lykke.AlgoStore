using System.IO;
using System.Linq;
using Lykke.AlgoStore.Core.Settings;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Lykke.AlgoStore.Tests.Infrastructure
{
    public static class SettingsMock
    {
        private static readonly string FileName = "appsettings.Development.json";

        public static IReloadingManager<AppSettings> InitConfigurationFromFile()
        {
            var config = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
            config.Providers.First().Set("SettingsUrl", "appsettings.Development.json");
            config.Providers.First().Set("ASPNETCORE_ENVIRONMENT", "Development");
            return config.LoadSettings<AppSettings>();
        }

        public static IReloadingManager<AppSettings> InitMockConfiguration()
        {
            var reloadingMock = new Mock<IReloadingManager<AppSettings>>();
            reloadingMock.Setup(x => x.CurrentValue).Returns
                (
                    new AppSettings
                    {
                        AlgoApi = new AlgoApiSettings
                        {
                            Db = new DbSettings
                            {
                                TableStorageConnectionString = "UseDevelopmentStorage=true",
                                LogsConnectionString = "UseDevelopmentStorage=true"
                            }
                        }
                    }
                );
            return reloadingMock.Object;
        }

        private static IReloadingManager<AppSettings> InitConfig()
        {
            return File.Exists(FileName) ? InitConfigurationFromFile() : InitMockConfiguration();
        }

        public static IReloadingManager<string> GetTableStorageConnectionString()
        {
            var config = InitConfig();

            return config.ConnectionString(x => x.AlgoApi.Db.TableStorageConnectionString);
        }

        public static IReloadingManager<string> GetLogsConnectionString()
        {
            var config = InitConfig();

            return config.ConnectionString(x => x.AlgoApi.Db.LogsConnectionString);
        }

        public static IReloadingManager<string> GetKubeBasicAuthenticationValue()
        {
            var config = InitConfig();

            return config.ConnectionString(x => x.AlgoApi.Kubernetes.BasicAuthenticationValue);
        }
    }
}
