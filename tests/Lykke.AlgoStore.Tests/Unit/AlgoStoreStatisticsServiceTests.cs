using System;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Statistics.Client;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoStoreStatisticsServiceTests
    {
        private readonly string _instanceId = Guid.NewGuid().ToString();
        private readonly string _clientId = Guid.NewGuid().ToString();

        private const string TradedAsset = "BTC";
        private const string AssetPair = "BTCUSD";

        [Test]
        public void GetStatisticsSummary()
        {
            var statisticsRepo = Given_Correct_StatisticsRepository();
            var algoInstanceRepo = Given_Correct_AlgoClientInstanceRepository();
            IStatisticsClient statisticsClientMock = Given_Correct_StatisticsClient();

            var statisticsService =
                Given_Correct_AlgoStoreStatisticsService(statisticsRepo, algoInstanceRepo, statisticsClientMock);

            var result =
                When_Invoke_GetStatisticsSummaryAsync(statisticsService, _clientId, _instanceId, out Exception ex);
            Then_Exception_Should_BeNull(ex);
            Then_Object_Should_NotBeNull(result);
        }

        [Test]
        public void UpdateStatisticsSummary()
        {
            var statisticsRepo = Given_Correct_StatisticsRepository();
            var algoInstanceRepo = Given_Correct_AlgoClientInstanceRepository();
            IStatisticsClient statisticsClientMock = Given_Correct_StatisticsClient();

            var statisticsService =
                Given_Correct_AlgoStoreStatisticsService(statisticsRepo, algoInstanceRepo, statisticsClientMock);

            var result =
                When_Invoke_UpdateStatisticsSummaryAsync(statisticsService, _clientId, _instanceId, out Exception ex);
            Then_Exception_Should_BeNull(ex);
            Then_Object_Should_NotBeNull(result);
        }

        #region Private methods

        private static AlgoStoreStatisticsService Given_Correct_AlgoStoreStatisticsService(
            IStatisticsRepository statisticsRepository,
            IAlgoClientInstanceRepository algoClientInstanceRepository, IStatisticsClient statisticsClient)
        {
            return new AlgoStoreStatisticsService(statisticsRepository, algoClientInstanceRepository,
                statisticsClient, new LogMock());
        }

        private static IStatisticsRepository Given_Correct_StatisticsRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.GetSummaryAsync(It.IsAny<string>())).Returns((string algoId) =>
            {
                var summary = fixture.Build<StatisticsSummary>().Create();
                return Task.FromResult(summary);
            });

            result.Setup(repo => repo.CreateOrUpdateSummaryAsync(It.IsAny<StatisticsSummary>()))
                .Returns((StatisticsSummary summary) => Task.FromResult(summary));

            result.Setup(x => x.SummaryExistsAsync(It.IsAny<string>())).Returns(() => Task.FromResult(true));

            return result.Object;
        }

        private static IAlgoClientInstanceRepository Given_Correct_AlgoClientInstanceRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo => repo.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientId, string id) =>
                {
                    return Task.FromResult(fixture.Build<AlgoClientInstanceData>()
                        .With(a => a.ClientId, clientId)
                        .With(b => b.InstanceId, id)
                        .With(a => a.TradedAssetId, TradedAsset)
                        .With(a => a.AssetPairId, AssetPair)
                        .Create());
                });

            return result.Object;
        }

        private static IStatisticsClient Given_Correct_StatisticsClient()
        {
            var fixture = new Fixture();
            var result = new Mock<IStatisticsClient>();

            result.Setup(client => client.UpdateSummaryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            return result.Object;
        }

        private static StatisticsSummary When_Invoke_GetStatisticsSummaryAsync(IAlgoStoreStatisticsService service,
            string clientId, string instanceId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetStatisticsSummaryAsync(clientId, instanceId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static StatisticsSummary When_Invoke_UpdateStatisticsSummaryAsync(IAlgoStoreStatisticsService service,
            string clientId, string instanceId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.UpdateStatisticsSummaryAsync(clientId, instanceId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static void Then_Exception_Should_BeNull(Exception ex) => Assert.IsNull(ex);

        private static void Then_Object_Should_NotBeNull(StatisticsSummary data) => Assert.NotNull(data);

        #endregion
    }
}
