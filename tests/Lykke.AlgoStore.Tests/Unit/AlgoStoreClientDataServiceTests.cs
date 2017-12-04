using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoStoreClientDataServiceTests
    {
        [Fact]
        public void GetClientMetadata_Returns_Data()
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null);
            Exception exception;
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }
        [Fact]
        public void GetClientMetadata_Throws_Exception()
        {
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null);
            Exception exception;
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Then_Data_ShouldBe_Empty(data);
        }
        [Fact]
        public void CascadeDeleteClientMetadata_Returns_Ok()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var externalClient = Given_Correct_ExternalClient_WithStatus(AlgoRuntimeStatuses.NotFound);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, externalClient);
            Exception exception;
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_Null(exception);
        }
        [Fact]
        public void CascadeDeleteClientMetadata_Returns_ImageExists()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var externalClient = Given_Correct_ExternalClient_WithStatus(AlgoRuntimeStatuses.Running);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, externalClient);
            Exception exception;
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }
        [Fact]
        public void CascadeDeleteClientMetadata_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var externalClient = Given_Correct_ExternalClient_WithStatus(AlgoRuntimeStatuses.Success);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, externalClient);
            Exception exception;
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }
        [Fact]
        public void CascadeDeleteClientMetadata_Blob_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Error_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var externalClient = Given_Correct_ExternalClient_WithStatus(AlgoRuntimeStatuses.Success);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, externalClient);
            Exception exception;
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }
        [Fact]
        public void CascadeDeleteClientMetadata_Runtime_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Error_AlgoRuntimeDataRepositoryMock();
            var externalClient = Given_Correct_ExternalClient_WithStatus(AlgoRuntimeStatuses.Success);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, externalClient);
            Exception exception;
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }
        [Fact]
        public void CascadeDeleteClientMetadata_ExternalClient_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var externalClient = Given_Error_ExternalClient();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, externalClient);
            Exception exception;
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }
        [Fact]
        public void SaveClientMetadata_Returns_Ok()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null);
            Exception exception;
            When_Invoke_SaveClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_Null(exception);
        }
        [Fact]
        public void SaveClientMetadata_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null);
            Exception exception;
            When_Invoke_SaveClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }


        private static AlgoStoreClientDataService Given_AlgoStoreClientDataService(
            IAlgoMetaDataRepository repo,
            IAlgoBlobRepository<byte[]> blobRepo,
            IAlgoRuntimeDataRepository runtimeDataRepository,
            IDeploymentApiReadOnlyClient externalClient)
        {
            return new AlgoStoreClientDataService(repo, null, runtimeDataRepository, null, blobRepo, null, externalClient, new LogMock());
        }
        private static AlgoClientMetaData When_Invoke_GetClientMetadata(AlgoStoreClientDataService service, string clientId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetClientMetadata(clientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }
        private static void When_Invoke_CascadeDeleteClientMetadata(AlgoStoreClientDataService service, string clientId, AlgoMetaData data, out Exception exception)
        {
            exception = null;
            try
            {
                service.CascadeDeleteClientMetadata(clientId, data).Wait();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
        private static void When_Invoke_SaveClientMetadata(AlgoStoreClientDataService service, string clientId, AlgoMetaData data, out Exception exception)
        {
            exception = null;
            try
            {
                service.SaveClientMetadata(clientId, data).Wait();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
        private static void Then_Data_ShouldNotBe_Empty(AlgoClientMetaData data)
        {
            Assert.NotNull(data);
            Assert.NotEmpty(data.AlgoMetaData);
        }
        private static void Then_Data_ShouldBe_Empty(AlgoClientMetaData data)
        {
            Assert.Null(data);
        }
        private static void Then_Exception_ShouldBe_Null(Exception exception)
        {
            Assert.Null(exception);
        }
        private static void Then_Exception_ShouldBe_ServiceException(Exception exception)
        {
            Exception temp = exception;

            var aggr = exception as AggregateException;
            if (aggr != null)
                temp = aggr.InnerExceptions[0];

            Assert.NotNull(temp);
            var serviceException = temp as AlgoStoreException;
            Assert.NotNull(serviceException);
        }
        private static IAlgoMetaDataRepository Given_Correct_AlgoMetaDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoMetaDataRepository>();
            result.Setup(repo => repo.DeleteAlgoMetaData(It.IsAny<AlgoClientMetaData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.GetAllClientAlgoMetaData(It.IsAny<string>()))
                .Returns((string clientId) => { return Task.FromResult(fixture.Build<AlgoClientMetaData>().With(a => a.ClientId, clientId).Create()); });
            result.Setup(repo => repo.GetAlgoMetaData(It.IsAny<string>()))
                .Returns((string id) =>
                {
                    var res = new AlgoClientMetaData();
                    res.ClientId = Guid.NewGuid().ToString();
                    res.AlgoMetaData = new List<AlgoMetaData>();
                    var data = fixture.Build<AlgoMetaData>()
                        .With(a => a.ClientAlgoId, id)
                        .Create();
                    res.AlgoMetaData.Add(data);

                    return Task.FromResult(res);
                });
            result.Setup(repo => repo.SaveAlgoMetaData(It.IsAny<AlgoClientMetaData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.DeleteAlgoMetaData(It.IsAny<AlgoClientMetaData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>())).Returns(Task.FromResult(true));

            return result.Object;
        }
        private static IAlgoBlobRepository<byte[]> Given_Correct_AlgoBlobRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoBlobRepository<byte[]>>();
            result.Setup(repo => repo.BlobExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            result.Setup(repo => repo.SaveBlobAsync(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.DeleteBlobAsync(It.IsAny<string>())).Returns(Task.FromResult(new byte[0]));

            return result.Object;
        }
        private static IAlgoRuntimeDataRepository Given_Correct_AlgoRuntimeDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoRuntimeDataRepository>();
            result.Setup(repo => repo.DeleteAlgoRuntimeData(It.IsAny<string>())).Returns(Task.FromResult(true));
            result.Setup(repo => repo.GetAlgoRuntimeData(It.IsAny<string>()))
                .Returns((string imageId) =>
                {
                    var res = new AlgoClientRuntimeData();
                    res.ClientAlgoId = Guid.NewGuid().ToString();
                    res.RuntimeData = new List<AlgoRuntimeData>();
                    var data = fixture.Build<AlgoRuntimeData>()
                        .With(a => a.ImageId, imageId)
                        .Create();
                    res.RuntimeData.Add(data);

                    return Task.FromResult(res);
                });
            result.Setup(repo => repo.GetAlgoRuntimeDataByAlgo(It.IsAny<string>()))
                .Returns((string algoId) =>
                {
                    var res = new AlgoClientRuntimeData();
                    res.ClientAlgoId = algoId;
                    res.RuntimeData = new List<AlgoRuntimeData>();
                    var data = fixture.Build<AlgoRuntimeData>()
                    .With(d => d.ImageId, "1")
                    .Create();
                    res.RuntimeData.Add(data);

                    return Task.FromResult(res);
                });
            result.Setup(repo => repo.SaveAlgoRuntimeData(It.IsAny<AlgoClientRuntimeData>())).Returns(Task.CompletedTask);

            return result.Object;
        }
        private static IAlgoMetaDataRepository Given_Error_AlgoMetaDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoMetaDataRepository>();
            result.Setup(repo => repo.DeleteAlgoMetaData(It.IsAny<AlgoClientMetaData>())).ThrowsAsync(new Exception("Delete"));
            result.Setup(repo => repo.GetAllClientAlgoMetaData(It.IsAny<string>())).ThrowsAsync(new Exception("GetAll"));
            result.Setup(repo => repo.GetAlgoMetaData(It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.SaveAlgoMetaData(It.IsAny<AlgoClientMetaData>())).ThrowsAsync(new Exception("Save"));
            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }
        private static IAlgoBlobRepository<byte[]> Given_Error_AlgoBlobRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoBlobRepository<byte[]>>();
            result.Setup(repo => repo.BlobExists(It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));
            result.Setup(repo => repo.SaveBlobAsync(It.IsAny<string>(), It.IsAny<byte[]>())).ThrowsAsync(new Exception("Save"));
            result.Setup(repo => repo.DeleteBlobAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Delete"));

            return result.Object;
        }
        private static IAlgoRuntimeDataRepository Given_Error_AlgoRuntimeDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoRuntimeDataRepository>();
            result.Setup(repo => repo.DeleteAlgoRuntimeData(It.IsAny<string>())).ThrowsAsync(new Exception("Delete"));
            result.Setup(repo => repo.GetAlgoRuntimeData(It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.GetAlgoRuntimeDataByAlgo(It.IsAny<string>())).ThrowsAsync(new Exception("GetByAlgo"));
            result.Setup(repo => repo.SaveAlgoRuntimeData(It.IsAny<AlgoClientRuntimeData>())).ThrowsAsync(new Exception("Save"));

            return result.Object;
        }
        public static IDeploymentApiReadOnlyClient Given_Correct_ExternalClient_WithStatus(AlgoRuntimeStatuses status)
        {
            var fixture = new Fixture();
            var result = new Mock<IDeploymentApiReadOnlyClient>();
            result.Setup(client => client.GetAlgoTestStatus(It.IsAny<long>())).Returns(Task.FromResult(status));

            return result.Object;
        }
        public static IDeploymentApiReadOnlyClient Given_Error_ExternalClient()
        {
            var fixture = new Fixture();
            var result = new Mock<IDeploymentApiReadOnlyClient>();
            result.Setup(client => client.GetAlgoTestStatus(It.IsAny<long>())).ThrowsAsync(new Exception("GetTestAlgoStatusUsingGET"));

            return result.Object;
        }
        private static AlgoMetaData Given_AlgoClientMetaData(string clientId)
        {
            var fixture = new Fixture();
            return fixture.Build<AlgoMetaData>().Create();
        }
    }
}
