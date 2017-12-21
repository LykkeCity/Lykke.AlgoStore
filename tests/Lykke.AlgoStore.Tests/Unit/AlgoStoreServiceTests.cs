using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoStoreServiceTests
    {
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

            result.Setup(repo => repo.GetAlgoMetaData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }

        private static void Then_Response_ShouldNotBe_Empty(bool response) => Assert.True(response);

        private static void Then_Exception_ShouldBe_Null(Exception exception) => Assert.Null(exception);

        private static bool When_Invoke_DeployImage(AlgoStoreService service, ManageImageData data, out Exception exception)
        {
            exception = null;
            try
            {
                return service.DeployImage(data).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return false;
        }

        private static AlgoStoreService Given_Correct_AlgoStoreServiceMock(
            IDeploymentApiClient deploymentApiClient,
            IAlgoBlobReadOnlyRepository blobRepo,
            IAlgoMetaDataReadOnlyRepository repo,
            IAlgoRuntimeDataRepository runtimeDataRepository)
        {
            return new AlgoStoreService(deploymentApiClient, new LogMock(), blobRepo, repo, runtimeDataRepository);
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
                client => client.BuildAlgoImageFromBinary(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync("1");
            result.Setup(client => client.CreateTestAlgo(It.IsAny<long>(), It.IsAny<string>())).Returns(Task.FromResult((long)1));

            return result.Object;
        }

        private static IAlgoBlobReadOnlyRepository Given_Correct_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository>();

            result.Setup(repo => repo.BlobExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            result.Setup(repo => repo.BlobExists(It.IsAny<string>())).Returns(Task.FromResult(true));

            return result.Object;
        }

        private static IAlgoRuntimeDataRepository Given_Correct_AlgoRuntimeDataRepositoryMock()
        {
            var result = new Mock<IAlgoRuntimeDataRepository>();

            result.Setup(repo => repo.SaveAlgoRuntimeData(It.IsAny<AlgoClientRuntimeData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.GetAlgoRuntimeData(It.IsAny<string>(), It.IsAny<string>()))
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

            result.Setup(repo => repo.BlobExists(It.IsAny<string>())).Returns(Task.FromResult(false));

            return result.Object;
        }

        private static IAlgoMetaDataReadOnlyRepository Given_Correct_AlgoMetaDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoMetaDataReadOnlyRepository>();

            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            result.Setup(repo => repo.GetAlgoMetaData(It.IsAny<string>(), It.IsAny<string>()))
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

            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            result.Setup(repo => repo.GetAlgoMetaData(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string id) =>
                {
                    var res = new AlgoClientMetaData();
                    res.ClientId = Guid.NewGuid().ToString();
                    res.AlgoMetaData = null;

                    return Task.FromResult(res);
                });

            return result.Object;
        }

        #endregion
    }
}
