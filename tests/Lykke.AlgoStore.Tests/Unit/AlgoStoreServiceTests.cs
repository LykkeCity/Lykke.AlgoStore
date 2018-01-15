using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoStoreServiceTests
    {
        #region Data Generation
        private static IEnumerable<Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses>> StartStatusData
        {
            get
            {
                return new List<Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses>>
                {
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Created, AlgoRuntimeStatuses.Started),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Stopped, AlgoRuntimeStatuses.Started),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Paused, AlgoRuntimeStatuses.Started),

                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Running, AlgoRuntimeStatuses.Started),

                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.NotFound, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Forbidden, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.InternalError, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Success, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Unauthorized, AlgoRuntimeStatuses.Unknown),
                };
            }
        }
        private static IEnumerable<Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses>> StopStatusData
        {
            get
            {
                return new List<Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses>>
                {
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Running, AlgoRuntimeStatuses.Stopped),

                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Stopped, AlgoRuntimeStatuses.Stopped),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Created, AlgoRuntimeStatuses.Deployed),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Paused, AlgoRuntimeStatuses.Paused),

                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.NotFound, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Forbidden, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.InternalError, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Success, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Unauthorized, AlgoRuntimeStatuses.Unknown),
                };
            }
        }
        #endregion
        [Test]
        public void DeployImage_Returns_True()
        {
            var data = Given_ManageImageData();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var deploymentApiClient = Given_Correct_DeploymentApiClientMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(deploymentApiClient, blobRepo, repo, runtimeRepo);

            var response = When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Response_ShouldNotBe_Empty(response);
        }

        [Test]
        public void DeployImage_WithInvalidAlgoMetaDataRepo_Throws_Exception()
        {
            var data = Given_ManageImageData();

            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var deploymentApiClient = Given_Correct_DeploymentApiClientMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(deploymentApiClient, blobRepo, repo, runtimeRepo);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void DeployImage_WithPartialyCorrectAlgoMetaDataRepo_Throws_Exception()
        {
            var data = Given_ManageImageData();

            var repo = Given_PartiallyCorrect_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var deploymentApiClient = Given_Correct_DeploymentApiClientMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(deploymentApiClient, blobRepo, repo, runtimeRepo);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void DeployImage_WithInvalidAlgoBlobRepo_Throws_Exception()
        {
            var data = Given_ManageImageData();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Error_AlgoBlobRepositoryMock();
            var deploymentApiClient = Given_Correct_DeploymentApiClientMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(deploymentApiClient, blobRepo, repo, runtimeRepo);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [TestCaseSource("StartStatusData")]
        public void StartTestImage_Returns_CorrectStatus(Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> statuses)
        {
            var data = Given_ManageImageData();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var deploymentApiClient = Given_Correct_DeploymentApiClientMock_WithStatus(statuses.Item1);
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(deploymentApiClient, null, repo, runtimeRepo);

            var response = When_Invoke_StartTest(service, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Response_ShouldBe_ExpectedStatus(response, statuses.Item2);
        }
        [TestCaseSource("StopStatusData")]
        public void StopTestImage_Returns_CorrectStatus(Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> statuses)
        {
            var data = Given_ManageImageData();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var deploymentApiClient = Given_Correct_DeploymentApiClientMock_WithStatus(statuses.Item1);
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(deploymentApiClient, null, repo, runtimeRepo);

            var response = When_Invoke_StopTest(service, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Response_ShouldBe_ExpectedStatus(response, statuses.Item2);
        }

        [Test]
        public void GetLog_Returns_Ok()
        {
            const string expectedLog = "TestLog";

            var data = Given_ManageImageData();

            var deploymentApiClient = Given_Correct_DeploymentApiClientMock_WithLog(expectedLog);
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(deploymentApiClient, null, null, runtimeRepo);

            var response = When_Invoke_GetLog(service, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Response_ShouldBe_ExpectedLog(response, expectedLog);
        }

        #region Private Methods

        private static void Then_Exception_ShouldBe_ServiceException(Exception exception)
        {
            var aggr = exception as AggregateException;
            Assert.NotNull(aggr);
            Assert.NotNull(aggr.InnerExceptions[0]);

            var serviceException = aggr.InnerExceptions[0] as AlgoStoreException;
            Assert.NotNull(serviceException);
        }

        private static IAlgoMetaDataReadOnlyRepository Given_Error_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoMetaDataReadOnlyRepository>();

            result.Setup(repo => repo.GetAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }

        private static void Then_Response_ShouldNotBe_Empty(bool response) => Assert.True(response);

        private static void Then_Exception_ShouldBe_Null(Exception exception) => Assert.Null(exception);

        private static bool When_Invoke_DeployImage(AlgoStoreService service, ManageImageData data, out Exception exception)
        {
            exception = null;
            try
            {
                return service.DeployImageAsync(data).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return false;
        }

        private static string When_Invoke_GetLog(AlgoStoreService service, ManageImageData data, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetTestLogAsync(data).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return string.Empty;
        }
        private static AlgoStoreService Given_Correct_AlgoStoreServiceMock(
            IDeploymentApiClient deploymentApiClient,
            IAlgoBlobReadOnlyRepository blobRepo,
            IAlgoMetaDataReadOnlyRepository repo,
            IAlgoRuntimeDataRepository runtimeDataRepository)
        {
            return new AlgoStoreService(deploymentApiClient, new LogMock(), blobRepo, repo, runtimeDataRepository, null, null);
        }

        private static ManageImageData Given_ManageImageData()
        {
            var fixture = new Fixture();

            return fixture.Build<ManageImageData>().Create();
        }

        private static IDeploymentApiClient Given_Correct_DeploymentApiClientMock()
        {
            var result = new Mock<IDeploymentApiClient>();

            result.Setup(
                client => client.BuildAlgoImageFromBinaryAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync("1");
            result.Setup(client => client.CreateTestAlgoAsync(It.IsAny<long>(), It.IsAny<string>())).Returns(Task.FromResult((long)1));

            return result.Object;
        }
        private static IDeploymentApiClient Given_Correct_DeploymentApiClientMock_WithStatus(ClientAlgoRuntimeStatuses status)
        {
            var result = new Mock<IDeploymentApiClient>();

            result.Setup(client => client.GetAlgoTestAdministrativeStatusAsync(It.IsAny<long>())).Returns(Task.FromResult(status));
            result.Setup(client => client.CreateTestAlgoAsync(It.IsAny<long>(), It.IsAny<string>())).Returns(Task.FromResult((long)1));
            result.Setup(client => client.StartTestAlgoAsync(It.IsAny<long>())).Returns(Task.FromResult(true));
            result.Setup(client => client.StopTestAlgoAsync(It.IsAny<long>())).Returns(Task.FromResult(true));

            return result.Object;
        }
        private static IDeploymentApiClient Given_Correct_DeploymentApiClientMock_WithLog(string log)
        {
            var result = new Mock<IDeploymentApiClient>();

            result.Setup(client => client.GetTestAlgoLogAsync(It.IsAny<long>())).Returns(Task.FromResult(log));

            return result.Object;
        }

        private static IAlgoBlobReadOnlyRepository Given_Correct_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository>();

            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));

            return result.Object;
        }

        private static IAlgoRuntimeDataRepository Given_Correct_AlgoRuntimeDataRepositoryMock()
        {
            var result = new Mock<IAlgoRuntimeDataRepository>();

            result.Setup(repo => repo.SaveAlgoRuntimeDataAsync(It.IsAny<AlgoClientRuntimeData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.GetAlgoRuntimeDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientId, string algoId) =>
                {
                    var res = new AlgoClientRuntimeData();
                    res.AlgoId = algoId;
                    res.ClientId = clientId;
                    res.ImageId = 1;

                    return Task.FromResult(res);
                });

            return result.Object;
        }

        private static IAlgoBlobReadOnlyRepository Given_Error_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository>();

            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(false));

            return result.Object;
        }

        private static IAlgoMetaDataReadOnlyRepository Given_Correct_AlgoMetaDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoMetaDataReadOnlyRepository>();

            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            result.Setup(repo => repo.GetAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientId, string id) =>
                {
                    var res = new AlgoClientMetaData();
                    res.ClientId = clientId;
                    res.AlgoMetaData = new List<AlgoMetaData>();
                    var data = fixture.Build<AlgoMetaData>()
                        .With(a => a.AlgoId, id)
                        .Create();
                    res.AlgoMetaData.Add(data);

                    return Task.FromResult(res);
                });

            return result.Object;
        }

        private static IAlgoMetaDataReadOnlyRepository Given_PartiallyCorrect_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoMetaDataReadOnlyRepository>();

            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            result.Setup(repo => repo.GetAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string id) =>
                {
                    var res = new AlgoClientMetaData();
                    res.ClientId = Guid.NewGuid().ToString();
                    res.AlgoMetaData = null;

                    return Task.FromResult(res);
                });

            return result.Object;
        }

        private static string When_Invoke_StartTest(AlgoStoreService service, ManageImageData data, out Exception exception)
        {
            exception = null;
            try
            {
                return service.StartTestImageAsync(data).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return string.Empty;
        }

        private static void Then_Response_ShouldBe_ExpectedStatus(string response, AlgoRuntimeStatuses expectedStatus)
        {
            Assert.AreEqual(response, expectedStatus.ToUpperText());
        }
        private static string When_Invoke_StopTest(AlgoStoreService service, ManageImageData data, out Exception exception)
        {
            exception = null;
            try
            {
                return service.StopTestImageAsync(data).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return string.Empty;
        }

        private static void Then_Response_ShouldBe_ExpectedLog(string response, string expectedLog)
        {
            Assert.AreEqual(response, expectedLog);
        }
        #endregion
    }
}
