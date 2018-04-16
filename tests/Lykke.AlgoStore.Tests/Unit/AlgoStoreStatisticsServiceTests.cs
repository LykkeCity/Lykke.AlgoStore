using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.PersonalData.Contract;
using Microsoft.Rest;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoStoreStatisticsServiceTests
    {
        private readonly string InstanceId = Guid.NewGuid().ToString();
        private readonly string ClientId = Guid.NewGuid().ToString();

        private const string TradedAsset = "BTC";
        private const string QuotingAsset = "USD";
        private const string AssetPair = "BTCUSD";

        [Test]
        public void GetStatisticsSummary()
        {
            var statisticsRepo = Given_Correct_StatisticsRepository();
            var algoInstanceRepo = Given_Correct_AlgoClientInstanceRepository();
            var walletBalanceService = Given_Customized_WalletBalanceServiceMock();
            var assetsService = Given_Customized_AssetServiceMock();
            var statisticsService = Given_Correct_AlgoStoreStatisticsService(statisticsRepo, algoInstanceRepo,
                walletBalanceService, assetsService);

            var result = When_Invoke_GetStatisticsSummaryAsync(statisticsService, ClientId, InstanceId, out Exception ex);
            Then_Exception_Should_BeNull(ex);
            Then_Object_Should_NotBeNull(result);
        }

        [Test]
        public void UpdateStatisticsSummary()
        {
            var statisticsRepo = Given_Correct_StatisticsRepository();
            var algoInstanceRepo = Given_Correct_AlgoClientInstanceRepository();
            var walletBalanceService = Given_Customized_WalletBalanceServiceMock();
            var assetsService = Given_Customized_AssetServiceMock();
            var statisticsService = Given_Correct_AlgoStoreStatisticsService(statisticsRepo, algoInstanceRepo,
                walletBalanceService, assetsService);

            var result = When_Invoke_UpdateStatisticsSummaryAsync(statisticsService, ClientId, InstanceId, out Exception ex);
            Then_Exception_Should_BeNull(ex);
            Then_Object_Should_NotBeNull(result);
        }

        #region Private methods

        private static AlgoStoreStatisticsService Given_Correct_AlgoStoreStatisticsService(IStatisticsRepository statisticsRepository,
            IAlgoClientInstanceRepository algoClientInstanceRepository,
            IWalletBalanceService walletBalanceService, IAssetsService assetsService)
        {
            return new AlgoStoreStatisticsService(statisticsRepository, algoClientInstanceRepository, walletBalanceService, assetsService, new LogMock());
        }

        private static IStatisticsRepository Given_Correct_StatisticsRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.GetSummaryAsync(It.IsAny<string>())).Returns((string algoId) =>
            {
                var comment = fixture.Build<StatisticsSummary>()
                    .Create();
                return Task.FromResult(comment);
            });

            result.Setup(repo => repo.CreateOrUpdateSummaryAsync(It.IsAny<StatisticsSummary>())).Returns((StatisticsSummary summary) => Task.FromResult(summary));

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
                        .With(a => a.TradedAsset, TradedAsset)
                        .With(a => a.AssetPair, AssetPair)
                        .Create());
                });

            return result.Object;
        }

        private static IWalletBalanceService Given_Customized_WalletBalanceServiceMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IWalletBalanceService>();

            result.Setup(service => service.GetTotalWalletBalanceInBaseAssetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AssetPair>()))
                .ReturnsAsync(100);

            result.Setup(service => service.GetWalletBalancesAsync(It.IsAny<string>(), It.IsAny<AssetPair>()))
                .ReturnsAsync(new List<ClientBalanceResponseModel>
                {
                    fixture.Build<ClientBalanceResponseModel>()
                        .With(w => w.AssetId, TradedAsset)
                        .Create(),

                    fixture.Build<ClientBalanceResponseModel>()
                        .With(w => w.AssetId, QuotingAsset)
                        .Create()
                });


            return result.Object;
        }

        private static IAssetsService Given_Customized_AssetServiceMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAssetsService>();

            result.Setup(service => service.AssetPairGetWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpOperationResponse<AssetPair>
                {
                    Body = fixture.Build<AssetPair>()
                        .With(pair => pair.IsDisabled, false)
                        .Create(),
                    Response = new HttpResponseMessage(HttpStatusCode.OK)
                });

            result.Setup(service => service.AssetGetWithHttpMessagesAsync(It.IsAny<string>(),
                    It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpOperationResponse<Asset>
                {
                    Body = fixture.Build<Asset>()
                        .With(asset => asset.IsDisabled, false)
                        .With(asset => asset.Id, TradedAsset)
                        .Create(),
                    Response = new HttpResponseMessage(HttpStatusCode.OK)
                });

            return result.Object;
        }

        private static StatisticsSummary When_Invoke_GetStatisticsSummaryAsync(IAlgoStoreStatisticsService service, string clientId, string instanceId, out Exception exception)
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

        private static StatisticsSummary When_Invoke_UpdateStatisticsSummaryAsync(IAlgoStoreStatisticsService service, string clientId, string instanceId, out Exception exception)
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

        private static void Then_Exception_Should_BeNull(Exception ex)
        {
            Assert.IsNull(ex);
        }

        private static void Then_Object_Should_NotBeNull(StatisticsSummary data)
        {
            Assert.NotNull(data);
        }
        #endregion
    }
}
