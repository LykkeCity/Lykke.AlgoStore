using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AzureStorage.Blob;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoStoreClientDataServiceTests
    {
        private static readonly string BlobKey = "TestKey";
        private static readonly string AlogId = "AlgoId123";
        private static readonly string BlobAlgoString = "testString";
        private static readonly byte[] BlobBytes = Encoding.Unicode.GetBytes(BlobKey);

        #region Data Generation
        private static IEnumerable<Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses>> StatusesData
        {
            get
            {
                return new List<Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses>>
                {
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Created, AlgoRuntimeStatuses.Deployed),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Forbidden, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.InternalError, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.NotFound, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Paused, AlgoRuntimeStatuses.Paused),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Running, AlgoRuntimeStatuses.Started),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Stopped, AlgoRuntimeStatuses.Stopped),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Success, AlgoRuntimeStatuses.Unknown),
                    new Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> (ClientAlgoRuntimeStatuses.Unauthorized, AlgoRuntimeStatuses.Unknown),
                };
            }
        }
        #endregion

        [Test]
        public void SaveAlgoAsBinary_Test()
        {
            var algoClientMetaDataRepo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepository = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(algoClientMetaDataRepo, blobRepository, null, null);
            var uploadBinaryModel = Given_UploadAlgoBinaryData_Model();
            When_Invoke_SaveAlgoAsBinary(service, uploadBinaryModel);
            ThenAlgo_Binary_ShouldExist(uploadBinaryModel.AlgoId, blobRepository);
        }

        [Test]
        public void SaveAlgoAsString_Test()
        {
            var algoClientMetaDataRepo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepository = Given_AlgoBinary_InMemory_Storage_Repository();
            var service = Given_AlgoStoreClientDataService(algoClientMetaDataRepo, blobRepository, null, null);

            When_Invoke_SaveAlgoAsString(service);
            ThenAlgo_StringShouldExist(blobRepository);
        }

        [TestCaseSource("StatusesData")]
        public void GetClientMetadata_Returns_Data(Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> statuses)
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var deploymentClient = Given_Correct_DeploymentApiClientMock(statuses.Item1);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, deploymentClient);
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
            Then_Data_ShouldBe_WithCorrectStatus(data, statuses.Item2);
        }
        [Test]
        public void GetClientMetadata_Returns_DataWithStatus()
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var deploymentClient = Given_Correct_DeploymentApiClientMock(ClientAlgoRuntimeStatuses.Created);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, deploymentClient);
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetClientMetadata_Throws_Exception()
        {
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var deploymentClient = Given_Correct_DeploymentApiClientMock(ClientAlgoRuntimeStatuses.Created);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, deploymentClient);
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void CascadeDeleteClientMetadata_Returns_Ok()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Null_AlgoRuntimeDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, null);
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
        }

        [Test]
        public void CascadeDeleteClientMetadata_Returns_ImageExists()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, null);
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void CascadeDeleteClientMetadata_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Null_AlgoRuntimeDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, null);
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void CascadeDeleteClientMetadata_Blob_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Error_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Null_AlgoRuntimeDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, null);
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void CascadeDeleteClientMetadata_Runtime_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Error_AlgoRuntimeDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, null);
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveClientMetadata_Returns_Ok()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null);
            When_Invoke_SaveClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_Null(exception);
        }

        [Test]
        public void SaveClientMetadata_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null);
            When_Invoke_SaveClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        #region Private Methods

        private static void ThenAlgo_StringShouldExist(IAlgoBlobRepository blobRepository)
        {
            var blob = blobRepository.GetBlobStringAsync(AlogId).Result;
            Assert.True(blob == BlobAlgoString);
        }

        private static void ThenAlgo_Binary_ShouldExist(string algoId, IAlgoBlobRepository blobRepository)
        {
            var blobExists = blobRepository.BlobExists(algoId).Result;
            Assert.True(blobExists);
        }

        private static UploadAlgoBinaryData Given_UploadAlgoBinaryData_Model()
        {
            var binaryFile = new Mock<IFormFile>();
            binaryFile.Setup(s => s.OpenReadStream()).Returns(new MemoryStream(BlobBytes));
            var model = new UploadAlgoBinaryData { AlgoId = AlogId, Data = binaryFile.Object };
            return model;
        }

        private static void When_Invoke_SaveAlgoAsString(AlgoStoreClientDataService service)
        {
            service.SaveAlgoAsString(AlogId, BlobAlgoString).Wait();
        }

        private static void When_Invoke_SaveAlgoAsBinary(AlgoStoreClientDataService service, UploadAlgoBinaryData model)
        {
            service.SaveAlgoAsBinary(model).Wait();
        }

        private static AlgoStoreClientDataService Given_AlgoStoreClientDataService(
            IAlgoMetaDataRepository repo,
            IAlgoBlobRepository blobRepo,
            IAlgoRuntimeDataReadOnlyRepository runtimeDataRepository,
            IDeploymentApiReadOnlyClient deploymentClient)
        {
            return new AlgoStoreClientDataService(repo, runtimeDataRepository, blobRepo, deploymentClient, new LogMock());
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
            Assert.IsNotEmpty(data.AlgoMetaData);
        }
        private static void Then_Data_ShouldBe_WithCorrectStatus(AlgoClientMetaData data, AlgoRuntimeStatuses status)
        {
            Assert.AreEqual(data.AlgoMetaData[0].Status, status.ToUpperText());
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

        private static IAlgoBlobRepository Given_Correct_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository>();
            result.Setup(repo => repo.BlobExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            result.Setup(repo => repo.SaveBlobAsync(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.DeleteBlobAsync(It.IsAny<string>())).Returns(Task.FromResult(new byte[0]));

            return result.Object;
        }

        private IAlgoBlobRepository Given_AlgoBinary_InMemory_Storage_Repository()
        {
            return new AlgoBlobRepository(new AzureBlobInMemory());
        }

        private static IAlgoRuntimeDataReadOnlyRepository Given_Correct_AlgoRuntimeDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoRuntimeDataReadOnlyRepository>();
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

            return result.Object;
        }

        private static IAlgoRuntimeDataReadOnlyRepository Given_Null_AlgoRuntimeDataRepositoryMock()
        {
            var result = new Mock<IAlgoRuntimeDataReadOnlyRepository>();
            result.Setup(repo => repo.GetAlgoRuntimeData(It.IsAny<string>()))
                .Returns(Task.FromResult<AlgoClientRuntimeData>(null));
            result.Setup(repo => repo.GetAlgoRuntimeDataByAlgo(It.IsAny<string>()))
                .Returns(Task.FromResult<AlgoClientRuntimeData>(null));

            return result.Object;
        }

        private static IAlgoMetaDataRepository Given_Error_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoMetaDataRepository>();
            result.Setup(repo => repo.DeleteAlgoMetaData(It.IsAny<AlgoClientMetaData>())).ThrowsAsync(new Exception("Delete"));
            result.Setup(repo => repo.GetAllClientAlgoMetaData(It.IsAny<string>())).ThrowsAsync(new Exception("GetAll"));
            result.Setup(repo => repo.GetAlgoMetaData(It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.SaveAlgoMetaData(It.IsAny<AlgoClientMetaData>())).ThrowsAsync(new Exception("Save"));
            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }

        private static IAlgoBlobRepository Given_Error_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository>();
            result.Setup(repo => repo.BlobExists(It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));
            result.Setup(repo => repo.SaveBlobAsync(It.IsAny<string>(), It.IsAny<byte[]>())).ThrowsAsync(new Exception("Save"));
            result.Setup(repo => repo.DeleteBlobAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Delete"));

            return result.Object;
        }

        private static IAlgoRuntimeDataReadOnlyRepository Given_Error_AlgoRuntimeDataRepositoryMock()
        {
            var result = new Mock<IAlgoRuntimeDataReadOnlyRepository>();
            result.Setup(repo => repo.GetAlgoRuntimeData(It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.GetAlgoRuntimeDataByAlgo(It.IsAny<string>())).ThrowsAsync(new Exception("GetByAlgo"));

            return result.Object;
        }

        private static AlgoMetaData Given_AlgoClientMetaData()
        {
            var fixture = new Fixture();
            return fixture.Build<AlgoMetaData>().Create();
        }

        private static IDeploymentApiReadOnlyClient Given_Correct_DeploymentApiClientMock(ClientAlgoRuntimeStatuses status)
        {
            var result = new Mock<IDeploymentApiReadOnlyClient>();
            result.Setup(repo => repo.GetAlgoTestAdministrativeStatus(It.IsAny<long>())).Returns(Task.FromResult(status));

            return result.Object;
        }

        #endregion
    }
}
