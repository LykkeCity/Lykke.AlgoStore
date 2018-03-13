using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.KubernetesClient.Models;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.AlgoStore.TeamCityClient.Models;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using NUnit.Framework;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoStoreServiceTests
    {
        private static readonly Fixture Fixture = new Fixture();

        [Test]
        public void DeployImage_Returns_True()
        {
            var data = Given_ManageImageData();
            var publicRepo = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Correct_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service = Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, publicRepo);

            var response = When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Response_ShouldBe_True(response);
        }
        [Test]
        public void DeployImage_WithInvalidAlgoMetaDataRepo_Throws_Exception()
        {
            var data = Given_ManageImageData();

            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Correct_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service = Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, null);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }
        [Test]
        public void DeployImage_WithInvalidAlgoBlobRepo_Throws_Exception()
        {
            var data = Given_ManageImageData();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Error_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Correct_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service = Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, null);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void DeployImage_WithInvalidInstanceRepo_Throws_Exception()
        {
            var data = Given_ManageImageData();
            var publicRepo = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Error_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service = Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, publicRepo);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void DeployImage_WithPublicAlgoNotFound_Throws_Exception()
        {
            var data = Given_ManageImageData();
            var publicRepo = Given_NotPublic_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Error_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service = Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, publicRepo);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void DeployImage_WithTeamCity_Returns_Undefined()
        {
            var data = Given_ManageImageData();

            var publicRepo = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Correct_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Random");
            var service = Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, publicRepo);

            bool res = When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Response_ShouldBe_False(res);
        }
        [Test]
        public void GetLog_Returns_Ok()
        {
            const string expectedLog = "TestLog";

            var data = Given_TailLogData();

            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(expectedLog);
            var instanceRepo = Given_Correct_AlgoInstanceDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, null, null, instanceRepo, null, null, null);

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
        private static void Then_Response_ShouldBe_True(bool response) => Assert.True(response);
        private static void Then_Response_ShouldBe_False(bool response) => Assert.False(response);
        private static void Then_Exception_ShouldBe_Null(Exception exception) => Assert.Null(exception);
        private static void Then_Response_ShouldBe_ExpectedLog(string response, string expectedLog)
        {
            Assert.AreEqual(response, expectedLog);
        }

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
        private static string When_Invoke_GetLog(AlgoStoreService service, TailLogData data, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetTestTailLogAsync(data).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return string.Empty;
        }

        private static AlgoStoreService Given_Correct_AlgoStoreServiceMock(
            IKubernetesApiClient deploymentApiClient,
            IAlgoBlobReadOnlyRepository blobRepo,
            IAlgoMetaDataReadOnlyRepository repo,
            IAlgoClientInstanceRepository instanceDataRepository,
            IStorageConnectionManager storageConnectionManager,
            ITeamCityClient teamCityClient,
            IPublicAlgosRepository publicAlgosRepository)
        {
            return new AlgoStoreService(new LogMock(), blobRepo, repo, null, storageConnectionManager, teamCityClient, deploymentApiClient, instanceDataRepository, publicAlgosRepository);
        }

        private static ManageImageData Given_ManageImageData()
        {
            return Fixture.Build<ManageImageData>().Create();
        }
        private static TailLogData Given_TailLogData()
        {
            return Fixture.Build<TailLogData>().Create();
        }

        private static IKubernetesApiClient Given_Correct_KubernetesApiClientMock_WithLog(string log)
        {
            var result = new Mock<IKubernetesApiClient>();

            result.Setup(client => client.ListPodsByAlgoIdAsync(It.IsAny<string>())).ReturnsAsync(
                new List<Iok8skubernetespkgapiv1Pod>
                {
                    Fixture.Build<Iok8skubernetespkgapiv1Pod>().Create()
                });
            result.Setup(client => client.ReadPodLogAsync(It.IsAny<Iok8skubernetespkgapiv1Pod>(), It.IsAny<int>()))
                .ReturnsAsync(log);

            return result.Object;
        }

        private static IPublicAlgosRepository Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock()
        {
            var result = new Mock<IPublicAlgosRepository>();
            result.Setup(repo => repo.ExistsPublicAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
            {
                return Task.FromResult(true);
            });

            return result.Object;
        }

        private static IPublicAlgosRepository Given_NotPublic_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock()
        {
            var result = new Mock<IPublicAlgosRepository>();
            result.Setup(repo => repo.ExistsPublicAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
            {
                return Task.FromResult(false);
            });

            return result.Object;
        }

        private static IAlgoBlobReadOnlyRepository Given_Correct_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository>();

            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));

            return result.Object;
        }
        private static IAlgoBlobReadOnlyRepository Given_Error_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository>();

            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            return result.Object;
        }

        private static IAlgoClientInstanceRepository Given_Correct_AlgoInstanceDataRepositoryMock()
        {
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo =>
                    repo.ExistsAlgoInstanceDataWithAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            result.Setup(repo =>
                    repo.GetAlgoInstanceDataByAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Fixture.Build<AlgoClientInstanceData>().Create());

            return result.Object;
        }
        private static IAlgoClientInstanceRepository Given_Error_AlgoInstanceDataRepositoryMock()
        {
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo =>
                    repo.ExistsAlgoInstanceDataWithAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("ExistsAlgoInstanceDataAsync"));

            result.Setup(repo =>
                    repo.GetAlgoInstanceDataByAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("GetAlgoInstanceDataAsync"));

            return result.Object;
        }


        private static IAlgoMetaDataReadOnlyRepository Given_Error_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoMetaDataReadOnlyRepository>();

            result.Setup(repo => repo.GetAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }
        private static IAlgoMetaDataReadOnlyRepository Given_Correct_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoMetaDataReadOnlyRepository>();

            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            result.Setup(repo => repo.GetAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientId, string id) =>
                {
                    var res = new AlgoClientMetaData();
                    res.ClientId = clientId;
                    res.AlgoMetaData = new List<AlgoMetaData>();
                    var data = Fixture.Build<AlgoMetaData>()
                        .With(a => a.AlgoId, id)
                        .Create();
                    res.AlgoMetaData.Add(data);

                    return Task.FromResult(res);
                });

            return result.Object;
        }

        private static IStorageConnectionManager Given_Correct_StorageConnectionManager()
        {
            var result = new Mock<IStorageConnectionManager>();

            result.Setup(repo => repo.GetData(It.IsAny<string>())).Returns(Fixture.Build<StorageConnectionData>().Create());

            return result.Object;
        }

        private static ITeamCityClient Given_Correct_TeamCityClient_WithState(string state)
        {
            var result = new Mock<ITeamCityClient>();

            result.Setup(repo => repo.StartBuild(It.IsAny<TeamCityClientBuildData>()))
                .ReturnsAsync(
                Fixture.Build<BuildBase>().With(b => b.State, state)
                .Create());

            return result.Object;
        }
        #endregion
    }
}
