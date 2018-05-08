using Lykke.AlgoStore.Api.Controllers;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Service.AlgoTrades.Client.AutorestClient.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Services;
using Assert = NUnit.Framework.Assert;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.AlgoTrades.Client;
using Lykke.AlgoStore.Service.AlgoTrades.Client.Models;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoTradesTests
    {
        private static readonly string _mockInstanceId = "1xd49482-2108-4b39-97ed-61ca1f4df410";
        private static readonly string _mockClientId = "113369482-2108-4b39-97ed-61ca1f4df410";
        private static readonly string _mockTradedAsset = "USD";
        private static readonly int _mockedItemsCount = 10;
        [Test]
        public async Task GetAlgoInstanceTrades_OkResult()
        {
            var algoTradesService = GetAlgoTradesServiceMock();
            AlgoStoreTradesController controller = new AlgoStoreTradesController(algoTradesService);

            var result = (await controller.GetAllTradesForAlgoInstanceAsync(_mockInstanceId)) as OkObjectResult;
            Assert.IsNotNull(result);

            var values = result.Value as IEnumerable<AlgoInstanceTradeResponseModel>;

            Assert.AreEqual(values.Count(), GetTradesResult().Count());
            Assert.AreEqual(values.FirstOrDefault().InstanceId, _mockInstanceId);
        }

        [Test]
        public void GetAlgoInstanceTrades_Service()
        {
            var repoMock = GetAlgoInstanceRepositoryMock();
            var clientMock = GetTradesClientResultMock();
            var logsMock = new Mock<ILog>().Object;
            var service = new AlgoStoreTradesService(logsMock, clientMock, repoMock, 1);

            var response = When_Invoke_GetTrades(service, _mockClientId, _mockInstanceId, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Assert.NotNull(response);
        }

        [Test]
        public void CheckValidParameters()
        {
            var repoMock = GetAlgoInstanceRepositoryMock();
            var clientMock = GetTradesClientResult_CheckParams_Mock();
            var logsMock = new Mock<ILog>().Object;
            var service = new AlgoStoreTradesService(logsMock, clientMock, repoMock, _mockedItemsCount);

            var response = When_Invoke_GetTrades(service, _mockClientId, _mockInstanceId, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Assert.NotNull(response);
        }

        [Test]
        public void GetAlgoInstanceTrades_Service_Exception_EmptyInstanceId()
        {
            var repoMock = GetAlgoInstanceRepositoryMock();
            var clientMock = GetTradesClientResultMock();
            var logsMock = new Mock<ILog>().Object;
            var service = new AlgoStoreTradesService(logsMock, clientMock, repoMock, 1);

            var response = When_Invoke_GetTrades(service, _mockClientId, null, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Assert.IsNull(response);
        }

        [Test]
        public void GetAlgoInstanceTrades_Service_Exception_ClientError()
        {
            var repoMock = GetAlgoInstanceRepositoryMock();
            var clientMock = GetTradesClientResult_Error_Mock();
            var logsMock = new Mock<ILog>().Object;
            var service = new AlgoStoreTradesService(logsMock, clientMock, repoMock, 1);

            var response = When_Invoke_GetTrades(service, _mockClientId, _mockInstanceId, out var exception);
            Then_Exception_ShouldBe_ServiceException_AlgoTradesClientError(exception);
            Assert.IsNull(response);
        }

        #region Helpers
        private static void Then_Exception_ShouldBe_ServiceException(Exception exception)
        {
            var aggr = exception as AggregateException;
            Assert.NotNull(aggr);
            Assert.NotNull(aggr.InnerExceptions[0]);

            var serviceException = aggr.InnerExceptions[0] as AlgoStoreException;
            Assert.NotNull(serviceException);
        }

        private static void Then_Exception_ShouldBe_ServiceException_AlgoTradesClientError(Exception exception)
        {
            var aggr = exception as AggregateException;
            Assert.NotNull(aggr);
            Assert.NotNull(aggr.InnerExceptions[0]);

            var serviceException = aggr.InnerExceptions[0] as AlgoStoreException;
            Assert.NotNull(serviceException);
        }

        private static IEnumerable<AlgoInstanceTradeResponseModel> When_Invoke_GetTrades(AlgoStoreTradesService service, string clientId, string instanceId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAllTradesForAlgoInstanceAsync(clientId, instanceId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return null;
        }

        private IAlgoStoreTradesService GetAlgoTradesServiceMock()
        {
            var service = new Mock<IAlgoStoreTradesService>();

            service.Setup(s => s.GetAllTradesForAlgoInstanceAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(GetTradesResult()));

            return service.Object;
        }

        private IAlgoClientInstanceRepository GetAlgoInstanceRepositoryMock()
        {
            var service = new Mock<IAlgoClientInstanceRepository>();

            service.Setup(s => s.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetInstance()));

            return service.Object;
        }

        private static void Then_Exception_ShouldBe_Null(Exception exception) => Assert.Null(exception);

        private IAlgoTradesClient GetTradesClientResult_Error_Mock()
        {
            var service = new Mock<IAlgoTradesClient>();

            service.Setup(s =>
                    s.GetAlgoInstanceTradesByTradedAsset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new AlgoInstanceTradeResponse()
                {
                    Error = new ErrorModel()
                    {
                        Message = "Invalid data"
                    }
                }));

            return service.Object;
        }

        private IAlgoTradesClient GetTradesClientResultMock()
        {
            var service = new Mock<IAlgoTradesClient>();

            service.Setup(s => s.GetAlgoInstanceTradesByTradedAsset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.FromResult(GetClientTestResult()));

            return service.Object;
        }

        private IAlgoTradesClient GetTradesClientResult_CheckParams_Mock()
        {
            var service = new Mock<IAlgoTradesClient>();

            service.Setup(s => s.GetAlgoInstanceTradesByTradedAsset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns((string instanceId, string tradedAsset, int maxNumber) =>
                {
                    Assert.AreEqual(tradedAsset, _mockTradedAsset);
                    Assert.AreEqual(instanceId, _mockInstanceId);
                    Assert.AreEqual(maxNumber, _mockedItemsCount);

                    return Task.FromResult(GetClientTestResult());
                });

            return service.Object;
        }

        private static IEnumerable<AlgoInstanceTradeResponseModel> GetTradesResult()
        {
            List<AlgoInstanceTradeResponseModel> resultList = new List<AlgoInstanceTradeResponseModel>();

            resultList.Add(new AlgoInstanceTradeResponseModel()
            {
                Amount = 5,
                TradedAssetName = "USD",
                AssetPair = "BTC/USD",
                Fee = 0.05,
                IsBuy = true,
                Price = 6500,
                InstanceId = _mockInstanceId
            });

            return resultList;
        }

        private static AlgoInstanceTradeResponse GetClientTestResult()
        {
            var result = GetTradesResult().ToList();
            return new AlgoInstanceTradeResponse()
            {
                Records = result
            };
        }

        private static AlgoClientInstanceData GetInstance()
        {
            AlgoClientInstanceData mockedResult = new AlgoClientInstanceData()
            {
                InstanceId = _mockInstanceId,
                ClientId = _mockClientId,
                InstanceName = "Test",
                TradedAsset = _mockTradedAsset
            };
            return mockedResult;
        }

        #endregion
    }
}
