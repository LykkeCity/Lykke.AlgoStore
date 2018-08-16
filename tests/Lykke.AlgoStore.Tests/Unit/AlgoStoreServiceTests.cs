using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Job.Stopping.Client.AutorestClient.Models;
using Lykke.AlgoStore.Job.Stopping.Client.Models.ResponseModels;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.AlgoStore.TeamCityClient.Models;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.Logging.Client.AutorestClient.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            var repo = Given_Correct_AlgoRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Correct_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service =
                Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, publicRepo, null, null);

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
            var service =
                Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, null, null, null);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }
        [Test]
        public void DeployImage_WithInvalidAlgoBlobRepo_Throws_Exception()
        {
            var data = Given_ManageImageData();

            var repo = Given_Correct_AlgoRepositoryMock();
            var blobRepo = Given_Error_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Correct_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service =
                Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, null, null, null);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void DeployImage_WithInvalidInstanceRepo_Throws_Exception()
        {
            var data = Given_ManageImageData();
            var publicRepo = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();

            var repo = Given_Correct_AlgoRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Error_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service =
                Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, publicRepo, null, null);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void DeployImage_WithPublicAlgoNotFound_Throws_Exception()
        {
            var data = Given_ManageImageData();
            var publicRepo = Given_NotPublic_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();

            var repo = Given_Correct_AlgoRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Error_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Queued");
            var service =
                Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, publicRepo, null, null);

            When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void DeployImage_WithTeamCity_Returns_Undefined()
        {
            var data = Given_ManageImageData();

            var publicRepo = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();

            var repo = Given_Correct_AlgoRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var kubernetesApiClient = Given_Correct_KubernetesApiClientMock_WithLog(string.Empty);
            var instanceDataRepository = Given_Correct_AlgoInstanceDataRepositoryMock();
            var connectionManager = Given_Correct_StorageConnectionManager();
            var teamCityClient = Given_Correct_TeamCityClient_WithState("Random");
            var service =
                Given_Correct_AlgoStoreServiceMock(kubernetesApiClient, blobRepo, repo, instanceDataRepository, connectionManager, teamCityClient, publicRepo, null, null);

            bool res = When_Invoke_DeployImage(service, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Response_ShouldBe_False(res);
        }

        [Test]
        public void GetLog_Returns_Data()
        {
            var apiReturnedLog = new List<UserLogResponse>
            {
                new UserLogResponse
                {
                    Date = DateTime.Parse("2018-01-01T12:00:00.123456789Z").ToUniversalTime(),
                    Message = "testlog"
                },
                new UserLogResponse
                {
                    Date = DateTime.Parse("2018-01-01T12:01:00.123456789Z").ToUniversalTime(),
                    Message = "testlog2"
                }
            };

            string[] expectedLog = new string[] { "[2018-01-01 12:00:00] testlog", "[2018-01-01 12:01:00] testlog2" };

            var data = Given_TailLogData();

            var userLogRepository = Given_Correct_UserLogRepositoryMock_WithLog(apiReturnedLog);
            var instanceRepo = Given_Correct_AlgoInstanceDataRepositoryMock();
            var service = Given_Correct_AlgoStoreServiceMock(null, null, null, instanceRepo, null, null, null, null, userLogRepository);

            var response = When_Invoke_GetLog(service, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
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

        private void Then_Exception_Should_Be_AggregateException(Exception exception)
        {
            Assert.IsInstanceOf<AggregateException>(exception);
        }

        private static void Then_Response_ShouldBe_True(bool response) => Assert.True(response);
        private static void Then_Response_ShouldBe_False(bool response) => Assert.False(response);
        private static void Then_Exception_Should_Exist(Exception exception) => Assert.NotNull(exception);
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
        private static string[] When_Invoke_GetLog(AlgoStoreService service, TailLogData data, out Exception exception)
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

            return new string[0];
        }

        private static AlgoStoreService Given_Correct_AlgoStoreServiceMock(
            IAlgoInstanceStoppingClient algoInstanceStoppingClient,
            IAlgoBlobReadOnlyRepository blobRepo,
            IAlgoReadOnlyRepository repo,
            IAlgoClientInstanceRepository instanceDataRepository,
            IStorageConnectionManager storageConnectionManager,
            ITeamCityClient teamCityClient,
            IPublicAlgosRepository publicAlgosRepository,
            IStatisticsRepository statisticsRepository,
            ILoggingClient loggingClient)
        {
            return new AlgoStoreService(new LogMock(), blobRepo, repo, storageConnectionManager, teamCityClient,
                algoInstanceStoppingClient, instanceDataRepository, publicAlgosRepository, statisticsRepository, loggingClient);
        }

        private static ManageImageData Given_ManageImageData()
        {
            return Fixture.Build<ManageImageData>().Create();
        }
        private static TailLogData Given_TailLogData()
        {
            return Fixture.Build<TailLogData>().Create();
        }

        private static IAlgoInstanceStoppingClient Given_Correct_KubernetesApiClientMock_WithLog(string log)
        {
            var result = new Mock<IAlgoInstanceStoppingClient>();

            result.Setup(client => client.GetPodsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PodsResponse
            {
                Records = new List<PodResponseModel>
                {
                    Fixture.Build<PodResponseModel>().Create()
                }
            });

            //result.Setup(client => client.ListPodsByAlgoIdAsync(It.IsAny<string>())).ReturnsAsync(
            //new List<Iok8skubernetespkgapiv1Pod>
            //{
            //        Fixture.Build<Iok8skubernetespkgapiv1Pod>().Create()
            //});

            return result.Object;
        }

        private static ILoggingClient Given_Correct_UserLogRepositoryMock_WithLog(List<UserLogResponse> logs)
        {
            var result = new Mock<ILoggingClient>();

            result.Setup(repo => repo.GetTailLog(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(logs);

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


        private static IAlgoReadOnlyRepository Given_Error_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoReadOnlyRepository>();

            result.Setup(repo => repo.GetAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.ExistsAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }

        private static IAlgoReadOnlyRepository Given_Correct_AlgoRepositoryMock()
        {
            var result = new Mock<IAlgoReadOnlyRepository>();

            result.Setup(repo => repo.ExistsAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            result.Setup(repo => repo.GetAlgoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientId, string id) =>
                {
                    var res = Fixture.Build<AlgoEntity>()
                        .With(a => a.RowKey, id)
                        .With(a => a.PartitionKey, clientId)
                        .Create();

                    return Task.FromResult(res as IAlgo);
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
