using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Settings;
using Lykke.Logs;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Api.Infrastructure.Managers
{
    internal static class LogManager
    {
        public static ILog CreateLogWithSlack(IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(settings.CurrentValue.SlackNotifications.AzureQueue, aggregateLogger);

            var dbLogConnectionStringManager = settings.Nested(x => x.AlgoApi.Db.LogsConnectionString);
            var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

            // Creating azure storage logger, which logs own messages to console log
            if (!string.IsNullOrEmpty(dbLogConnectionString) && !(dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}")))
            {
                var persistenceManager = new LykkeLogToAzureStoragePersistenceManager(
                    AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, AlgoStoreConstants.LogTableName, consoleLogger),
                    consoleLogger);

                var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);

                var azureStorageLogger = new LykkeLogToAzureStorage(
                    persistenceManager,
                    slackNotificationsManager,
                    consoleLogger);

                azureStorageLogger.Start();

                aggregateLogger.AddLog(azureStorageLogger);
            }

            return aggregateLogger;
        }
    }
}
