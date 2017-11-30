﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoStoreServiceTests
    {
        [Fact]
        public void DeployImage_Returns_True()
        {
            var data = Given_DeployImageData();

            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var deploymentApiClient = Given_Correct_DeploymentApiClientMock();
            var service = Given_Correct_AlgoStoreServiceMock(deploymentApiClient, blobRepo, repo);

            Exception exception;
            var response = When_Invoke_DeployImage(service, data, out exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Response_ShouldNotBe_Empty(response);
        }

        private static void Then_Response_ShouldNotBe_Empty(bool response) => Assert.True(response);

        private static void Then_Exception_ShouldBe_Null(Exception exception) => Assert.Null(exception);

        private static bool When_Invoke_DeployImage(AlgoStoreService service, DeployImageData data, out Exception exception)
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
            IApiDocumentation deploymentApiClient, 
            IAlgoBlobRepository<byte[]> blobRepo, IAlgoMetaDataRepository repo)
        {
            return new AlgoStoreService(deploymentApiClient, new LogMock(), blobRepo, repo);
        }

        private static DeployImageData Given_DeployImageData()
        {
            var fixture = new Fixture();

            return fixture.Build<DeployImageData>().Create();
        }

        private static IApiDocumentation Given_Correct_DeploymentApiClientMock()
        {
            var result = new Mock<IApiDocumentation>();

            result.Setup(
                client => client.BuildAlgoImageFromBinaryUsingPOSTWithHttpMessagesAsync(Stream.Null, String.Empty,
                    String.Empty, new Dictionary<string, List<string>>(), CancellationToken.None));

            return result.Object;
        }

        private static IAlgoBlobRepository<byte[]> Given_Correct_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository<byte[]>>();

            result.Setup(repo => repo.BlobExists(It.IsAny<string>())).Returns(Task.FromResult(true));

            return result.Object;
        }

        private static IAlgoMetaDataRepository Given_Correct_AlgoMetaDataRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoMetaDataRepository>();

            result.Setup(repo => repo.ExistsAlgoMetaData(It.IsAny<string>())).Returns(Task.FromResult(true));

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

            return result.Object;
        }
    }
}
