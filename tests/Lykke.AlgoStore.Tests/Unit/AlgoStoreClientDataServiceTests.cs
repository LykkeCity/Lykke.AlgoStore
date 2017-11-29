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

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoStoreClientDataServiceTests
    {
        [Fact]
        public void GetClientMetadata_Returns_Data()
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo);
            Exception exception;
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }
        [Fact]
        public void GetClientMetadata_Throws_Exception()
        {
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo);
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
            var service = Given_AlgoStoreClientDataService(repo);
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
            var service = Given_AlgoStoreClientDataService(repo);
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
            var service = Given_AlgoStoreClientDataService(repo);
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
            var service = Given_AlgoStoreClientDataService(repo);
            Exception exception;
            When_Invoke_SaveClientMetadata(service, clientId, data, out exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }


        private static AlgoStoreClientDataService Given_AlgoStoreClientDataService(IAlgoMetaDataRepository repo)
        {
            return new AlgoStoreClientDataService(repo, null, null, null, null, new LogMock());
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

            return result.Object;
        }
        private static AlgoMetaData Given_AlgoClientMetaData(string clientId)
        {
            var fixture = new Fixture();
            return fixture.Build<AlgoMetaData>().Create();
        }
    }
}
