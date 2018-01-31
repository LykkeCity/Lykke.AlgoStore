using System;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class ControllerTests
    {
        private static readonly Fixture Fixture = new Fixture();

        [Test]
        public void DeleteAlgoMetadataTest_ReturnSuccess()
        {
            string clientId = Guid.NewGuid().ToString();

            var data = Given_AlgoMetaData();
            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true),
                Given_RuntimeDataReadOnlyRepository_WithResult(false),
                Given_BlobRepository_WithResult(true),
                null
                );

            var deploymentClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
            deploymentClient = Given_DeploymentApiClient_WithResult(deploymentClient, true, true, true);
            var algoService = Given_AlgoStoreService(deploymentClient.Object, null, null,
                Given_RuntimeDataRepository_WithResult(true));

            var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

            Then_Exception_ShouldBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_RuntimeDataNotExists_Thows()
        {
            string clientId = Guid.NewGuid().ToString();

            var data = Given_AlgoMetaData();
            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(false),
                Given_RuntimeDataReadOnlyRepository_WithResult(false),
                Given_BlobRepository_WithResult(true),
                null
            );

            var deploymentClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
            deploymentClient = Given_DeploymentApiClient_WithResult(deploymentClient, true, true, true);
            var algoService = Given_AlgoStoreService(deploymentClient.Object, null, null,
                Given_RuntimeDataRepository_WithResult(true));

            var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_ImageStatus_NotFound_ReturnSuccess()
        {
            string clientId = Guid.NewGuid().ToString();

            var data = Given_AlgoMetaData();
            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true),
                Given_RuntimeDataReadOnlyRepository_WithResult(false),
                Given_BlobRepository_WithResult(true),
                null
            );

            var deploymentClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.NotFound);
            deploymentClient = Given_DeploymentApiClient_WithResult(deploymentClient, true, true, true);
            var algoService = Given_AlgoStoreService(deploymentClient.Object, null, null,
                Given_RuntimeDataRepository_WithResult(true));

            var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

            Then_Exception_ShouldBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_StopFail_Throws()
        {
            string clientId = Guid.NewGuid().ToString();

            var data = Given_AlgoMetaData();
            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true),
                Given_RuntimeDataReadOnlyRepository_WithResult(false),
                Given_BlobRepository_WithResult(true),
                null
            );

            var deploymentClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
            deploymentClient = Given_DeploymentApiClient_WithResult(deploymentClient, false, true, true);
            var algoService = Given_AlgoStoreService(deploymentClient.Object, null, null,
                Given_RuntimeDataRepository_WithResult(true));

            var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_DeleteTestFail_Throws()
        {
            string clientId = Guid.NewGuid().ToString();

            var data = Given_AlgoMetaData();
            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true),
                Given_RuntimeDataReadOnlyRepository_WithResult(false),
                Given_BlobRepository_WithResult(true),
                null
            );

            var deploymentClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
            deploymentClient = Given_DeploymentApiClient_WithResult(deploymentClient, true, false, true);
            var algoService = Given_AlgoStoreService(deploymentClient.Object, null, null,
                Given_RuntimeDataRepository_WithResult(true));

            var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_DeleteImageFail_Throws()
        {
            string clientId = Guid.NewGuid().ToString();

            var data = Given_AlgoMetaData();
            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true),
                Given_RuntimeDataReadOnlyRepository_WithResult(false),
                Given_BlobRepository_WithResult(true),
                null
            );

            var deploymentClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
            deploymentClient = Given_DeploymentApiClient_WithResult(deploymentClient, true, true, false);
            var algoService = Given_AlgoStoreService(deploymentClient.Object, null, null,
                Given_RuntimeDataRepository_WithResult(true));

            var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_DeleteRuntimeDataFail_Throws()
        {
            string clientId = Guid.NewGuid().ToString();

            var data = Given_AlgoMetaData();
            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true),
                Given_RuntimeDataReadOnlyRepository_WithResult(false),
                Given_BlobRepository_WithResult(true),
                null
            );

            var deploymentClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
            deploymentClient = Given_DeploymentApiClient_WithResult(deploymentClient, true, true, true);
            var algoService = Given_AlgoStoreService(deploymentClient.Object, null, null,
                Given_RuntimeDataRepository_WithResult(false));

            var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }

        #region Private Methods
        private static AlgoMetaData Given_AlgoMetaData()
        {
            return Fixture.Build<AlgoMetaData>().Create();
        }

        private static IAlgoMetaDataRepository Given_MetaDataRepository_Exists(bool exists)
        {
            var result = new Mock<IAlgoMetaDataRepository>();

            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(exists));
            result.Setup(repo => repo.DeleteAlgoMetaDataAsync(It.IsAny<AlgoClientMetaData>())).Returns(Task.CompletedTask);

            return result.Object;
        }
        private static IAlgoRuntimeDataReadOnlyRepository Given_RuntimeDataReadOnlyRepository_WithResult(bool returnNull)
        {
            var result = new Mock<IAlgoRuntimeDataReadOnlyRepository>();

            result.Setup(repo => repo.GetAlgoRuntimeDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientId, string algoId) =>
                {
                    if (returnNull)
                        return Task.FromResult<AlgoClientRuntimeData>(null);

                    var res = new AlgoClientRuntimeData();
                    res.ClientId = clientId;
                    res.AlgoId = algoId;
                    //res.ImageId = 2;
                    //res.BuildImageId = 1;

                    return Task.FromResult(res);
                });

            return result.Object;
        }
        private static Mock<IDeploymentApiClient> Given_DeploymentApiClient_WithStatus(Mock<IDeploymentApiClient> client, ClientAlgoRuntimeStatuses status)
        {
            var result = client;
            if (result == null)
                result = new Mock<IDeploymentApiClient>();

            result.Setup(c => c.GetAlgoTestAdministrativeStatusAsync(It.IsAny<long>())).Returns(Task.FromResult(status));

            return result;
        }
        private static Mock<IDeploymentApiClient> Given_DeploymentApiClient_WithResult(
            Mock<IDeploymentApiClient> client,
            bool resultStop,
            bool resultDeleteTest,
            bool resultDelete)
        {
            var result = client;
            if (result == null)
                result = new Mock<IDeploymentApiClient>();

            result.Setup(c => c.StopTestAlgoAsync(It.IsAny<long>())).Returns(Task.FromResult(resultStop));
            result.Setup(c => c.DeleteTestAlgoAsync(It.IsAny<long>())).Returns(Task.FromResult(resultDeleteTest));
            result.Setup(c => c.DeleteAlgoAsync(It.IsAny<long>())).Returns(Task.FromResult(resultDelete));

            return result;
        }
        private static IAlgoRuntimeDataRepository Given_RuntimeDataRepository_WithResult(bool success)
        {
            var result = new Mock<IAlgoRuntimeDataRepository>();

            result.Setup(repo => repo.DeleteAlgoRuntimeDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(success));

            return result.Object;
        }
        private static IAlgoBlobRepository Given_BlobRepository_WithResult(bool exists)
        {
            var result = new Mock<IAlgoBlobRepository>();

            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(exists));

            return result.Object;
        }

        private static IAlgoStoreClientDataService Given_ClientDataService(
            IAlgoMetaDataRepository metaDataRepository,
            IAlgoRuntimeDataReadOnlyRepository runtimeDataRepository,
            IAlgoBlobRepository blobRepository,
            IDeploymentApiReadOnlyClient deploymentClient)
        {
            var result = new AlgoStoreClientDataService(metaDataRepository, runtimeDataRepository, blobRepository,
                deploymentClient, null, null, null, null, null, new LogMock());

            return result;
        }

        private static IAlgoStoreService Given_AlgoStoreService(IDeploymentApiClient externalClient,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IAlgoRuntimeDataRepository algoRuntimeDataRepository)
        {
            var result = new AlgoStoreService(externalClient, new LogMock(), algoBlobRepository, algoMetaDataRepository,
                algoRuntimeDataRepository, null, null);
            return result;
        }

        private static async Task<Exception> When_Execute_Delete(
            string clientId,
            AlgoMetaData data,
            IAlgoStoreClientDataService clientDataService,
            IAlgoStoreService algoStoreService)
        {
            Exception res = null;

            try
            {
                var runtimeData = await clientDataService.ValidateCascadeDeleteClientMetadataRequestAsync(clientId, data);

                await algoStoreService.DeleteImageAsync(runtimeData);

                await clientDataService.DeleteMetadataAsync(clientId, data);
            }
            catch (Exception exception)
            {
                res = exception;
            }

            return res;
        }

        private static void Then_Exception_ShouldBeNull(Exception exception)
        {
            Assert.IsNull(exception);
        }

        private static void Then_Exception_ShouldNotBeNull(Exception exception)
        {
            Assert.IsNotNull(exception);
        }
        #endregion
    }
}
