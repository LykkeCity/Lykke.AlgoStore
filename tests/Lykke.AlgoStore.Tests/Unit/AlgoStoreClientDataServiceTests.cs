using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoStoreClientDataServiceTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private static readonly string BlobKey = "TestKey";
        private static readonly string AlogId = "AlgoId123";
        private static readonly byte[] BlobBytes = Encoding.Unicode.GetBytes(BlobKey);

        [Test]
        public void SaveAlgoAsBinary_Test()
        {
            var algoClientMetaDataRepo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepository = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(algoClientMetaDataRepo, blobRepository);
            var uploadBinaryModel = Given_UploadAlgoBinaryData_Model();
            When_Invoke_SaveAlgoAsBinary(service, uploadBinaryModel);
            ThenAlgo_Binary_ShouldExist(uploadBinaryModel.AlgoId, blobRepository);
        }

        [Test]
        public void SaveClientMetadata_Returns_Ok()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo);
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
            var service = Given_AlgoStoreClientDataService(repo, blobRepo);
            When_Invoke_SaveClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        #region Private Methods

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

        private static void When_Invoke_SaveAlgoAsBinary(AlgoStoreClientDataService service, UploadAlgoBinaryData model)
        {
            service.SaveAlgoAsBinary(ClientId, model).Wait();
        }

        private static AlgoStoreClientDataService Given_AlgoStoreClientDataService(
            IAlgoMetaDataRepository repo,
            IAlgoBlobRepository blobRepo)
        {
            return new AlgoStoreClientDataService(repo, blobRepo, new LogMock());
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
            result.Setup(repo => repo.GetAlgoMetaData(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientid, string id) =>
                {
                    var res = new AlgoClientMetaData();
                    res.ClientId = clientid;
                    res.AlgoMetaData = new List<AlgoMetaData>();
                    var data = fixture.Build<AlgoMetaData>()
                        .With(a => a.AlgoId, id)
                        .Create();
                    res.AlgoMetaData.Add(data);

                    return Task.FromResult(res);
                });
            result.Setup(repo => repo.SaveAlgoMetaData(It.IsAny<AlgoClientMetaData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.DeleteAlgoMetaData(It.IsAny<AlgoClientMetaData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

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

        private static IAlgoMetaDataRepository Given_Error_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoMetaDataRepository>();
            result.Setup(repo => repo.DeleteAlgoMetaData(It.IsAny<AlgoClientMetaData>())).ThrowsAsync(new Exception("Delete"));
            result.Setup(repo => repo.GetAllClientAlgoMetaData(It.IsAny<string>())).ThrowsAsync(new Exception("GetAll"));
            result.Setup(repo => repo.GetAlgoMetaData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.SaveAlgoMetaData(It.IsAny<AlgoClientMetaData>())).ThrowsAsync(new Exception("Save"));
            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }

        private static AlgoMetaData Given_AlgoClientMetaData()
        {
            var fixture = new Fixture();
            return fixture.Build<AlgoMetaData>().Create();
        }

        #endregion
    }
}
