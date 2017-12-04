using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using Xunit;
using Autofac;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IO;
using Lykke.AlgoStore.AzureRepositories.Repositories;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoStoreClientDataServiceTests
    {
        private static IContainer _ioc;
        private static string blobKey = "TestKey";
        private static string alogId = "AlgoId123";
        private static string blobAlgoString = "testString";
        private static byte[] blobBytes = Encoding.Unicode.GetBytes(blobKey);
        private static IAlgoBlobRepository<byte[]> _binaryRepo;
        private static IAlgoBlobRepository<string> _stringRepo;

        public AlgoStoreClientDataServiceTests()
        {
            SetUp();
        }

        private void SetUp()
        {
            var ioc = new ContainerBuilder();
            ioc.BindAzureReposInMemForTests();
            _ioc = ioc.Build();
            _binaryRepo = _ioc.ResolveNamed<IAlgoBlobRepository<byte[]>>("InMemoryRepo");
            _stringRepo = _ioc.Resolve<IAlgoBlobRepository<string>>();
        }

        [Fact]
        public void SaveAlgoAsBinary_Test()
        {
            var algoClientMetaDataRepo = Given_Correct_AlgoMetaDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(algoClientMetaDataRepo, null, null);
            var uploadBinaryModel = Given_UploadAlgoBinaryData_Model();
            When_Invoke_SaveAlgoAsBinary(service, uploadBinaryModel);
            ThenAlgo_Binary_ShouldExist(uploadBinaryModel.AlgoId);
        }

        private UploadAlgoBinaryData Given_UploadAlgoBinaryData_Model()
        {
            var binaryFile = new Mock<IFormFile>();
            binaryFile.Setup(s => s.OpenReadStream()).Returns(new MemoryStream(blobBytes));
            var model = new UploadAlgoBinaryData { AlgoId = alogId, Data = binaryFile.Object };
            return model;
        }

        [Fact]
        public void SaveAlgoAsString_Test()
        {
            var algoClientMetaDataRepo = Given_Correct_AlgoMetaDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(algoClientMetaDataRepo, null, null);

            When_Invoke_SaveAlgoAsString(service);
            ThenAlgo_StringShouldExist();
        }

        private static void When_Invoke_SaveAlgoAsString(AlgoStoreClientDataService service)
        {
            service.SaveAlgoAsString(alogId, blobAlgoString).Wait();
        }
        private static void ThenAlgo_StringShouldExist()
        {
            var blob = _stringRepo.GetBlobAsync(alogId).Result;
            Assert.True(blob == blobAlgoString);
        }
        private static void When_Invoke_SaveAlgoAsBinary(AlgoStoreClientDataService service, UploadAlgoBinaryData model)
        {
            service.SaveAlgoAsBinary(model).Wait();
        }
        private static void ThenAlgo_Binary_ShouldExist(string algoId)
        {
            var blobExists = _binaryRepo.BlobExists(algoId).Result;
            Assert.True(blobExists);
        }
       
        [Fact]
        public void GetClientMetadata_Returns_Data()
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null);
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
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null);
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
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo);
            Exception exception;
            When_Invoke_CascadeDeleteClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_Null(exception);
        }
        [Fact]
        public void CascadeDeleteClientMetadata_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData(clientId);
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo);
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
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo);
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
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo);
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
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null);
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
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null);
            Exception exception;
            When_Invoke_SaveClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }


        private static AlgoStoreClientDataService Given_AlgoStoreClientDataService(IAlgoMetaDataRepository repoMetaData, IAlgoBlobRepository<byte[]> blobRepo,  IAlgoRuntimeDataRepository runtimeRepo )
        {
            return new AlgoStoreClientDataService(repoMetaData, runtimeRepo, blobRepo ?? _binaryRepo, _stringRepo, new LogMock());
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

        private static AlgoDataRepository Given_Correct_AlgoDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<AlgoDataRepository>();
            result.Setup(repo => repo.SaveAlgoData(It.IsAny<AlgoData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.GetAlgoData(It.Is<string>(s=>s.Equals(alogId)))).Returns((string alogId) => { return Task.FromResult(fixture.Build<AlgoData>().With(a => a.ClientAlgoId, alogId).Create()); });

            return result.Object;
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
                    var data = fixture.Build<AlgoRuntimeData>().Create();
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
        private static AlgoMetaData Given_AlgoClientMetaData(string clientId)
        {
            var fixture = new Fixture();
            return fixture.Build<AlgoMetaData>().Create();
        }
    }
}
