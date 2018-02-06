using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.KubernetesClient.Models;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class ControllerTests
    {
        private static readonly Fixture fixture = new Fixture();

        [Test]
        public void DeleteAlgoMetadataTest_ReturnSuccess()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true);

            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true),
                instanceRepo,
                Given_BlobRepository_WithResult(true),
                null
                );

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_MetaDataNotExists_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true);

            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(false),
                instanceRepo,
                Given_BlobRepository_WithResult(true),
                null
            );

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        //[Test]
        //public void DeleteAlgoMetadataTest_ImageStatus_NotFound_ReturnSuccess()
        //{
        //    string clientId = Guid.NewGuid().ToString();

        //    var data = Given_AlgoMetaData();
        //    var clientDataService = Given_ClientDataService(
        //        Given_MetaDataRepository_Exists(true),
        //        Given_RuntimeDataReadOnlyRepository_WithResult(false),
        //        Given_BlobRepository_WithResult(true),
        //        null
        //    );

        //    var kubernetesClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.NotFound);
        //    kubernetesClient = Given_DeploymentApiClient_WithResult(kubernetesClient, true, true, true);
        //    var algoService = Given_AlgoStoreService(kubernetesClient.Object, null, null,
        //        Given_RuntimeDataRepository_WithResult(true));

        //    var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

        //    Then_Exception_ShouldBeNull(ex);
        //}
        //[Test]
        //public void DeleteAlgoMetadataTest_StopFail_Throws()
        //{
        //    string clientId = Guid.NewGuid().ToString();

        //    var data = Given_AlgoMetaData();
        //    var clientDataService = Given_ClientDataService(
        //        Given_MetaDataRepository_Exists(true),
        //        Given_RuntimeDataReadOnlyRepository_WithResult(false),
        //        Given_BlobRepository_WithResult(true),
        //        null
        //    );

        //    var kubernetesClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
        //    kubernetesClient = Given_DeploymentApiClient_WithResult(kubernetesClient, false, true, true);
        //    var algoService = Given_AlgoStoreService(kubernetesClient.Object, null, null,
        //        Given_RuntimeDataRepository_WithResult(true));

        //    var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

        //    Then_Exception_ShouldNotBeNull(ex);
        //}
        //[Test]
        //public void DeleteAlgoMetadataTest_DeleteTestFail_Throws()
        //{
        //    string clientId = Guid.NewGuid().ToString();

        //    var data = Given_AlgoMetaData();
        //    var clientDataService = Given_ClientDataService(
        //        Given_MetaDataRepository_Exists(true),
        //        Given_RuntimeDataReadOnlyRepository_WithResult(false),
        //        Given_BlobRepository_WithResult(true),
        //        null
        //    );

        //    var kubernetesClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
        //    kubernetesClient = Given_DeploymentApiClient_WithResult(kubernetesClient, true, false, true);
        //    var algoService = Given_AlgoStoreService(kubernetesClient.Object, null, null,
        //        Given_RuntimeDataRepository_WithResult(true));

        //    var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

        //    Then_Exception_ShouldNotBeNull(ex);
        //}
        //[Test]
        //public void DeleteAlgoMetadataTest_DeleteImageFail_Throws()
        //{
        //    string clientId = Guid.NewGuid().ToString();

        //    var data = Given_AlgoMetaData();
        //    var clientDataService = Given_ClientDataService(
        //        Given_MetaDataRepository_Exists(true),
        //        Given_RuntimeDataReadOnlyRepository_WithResult(false),
        //        Given_BlobRepository_WithResult(true),
        //        null
        //    );

        //    var kubernetesClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
        //    kubernetesClient = Given_DeploymentApiClient_WithResult(kubernetesClient, true, true, false);
        //    var algoService = Given_AlgoStoreService(kubernetesClient.Object, null, null,
        //        Given_RuntimeDataRepository_WithResult(true));

        //    var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

        //    Then_Exception_ShouldNotBeNull(ex);
        //}
        //[Test]
        //public void DeleteAlgoMetadataTest_DeleteRuntimeDataFail_Throws()
        //{
        //    string clientId = Guid.NewGuid().ToString();

        //    var data = Given_AlgoMetaData();
        //    var clientDataService = Given_ClientDataService(
        //        Given_MetaDataRepository_Exists(true),
        //        Given_RuntimeDataReadOnlyRepository_WithResult(false),
        //        Given_BlobRepository_WithResult(true),
        //        null
        //    );

        //    var kubernetesClient = Given_DeploymentApiClient_WithStatus(null, ClientAlgoRuntimeStatuses.Running);
        //    kubernetesClient = Given_DeploymentApiClient_WithResult(kubernetesClient, true, true, true);
        //    var algoService = Given_AlgoStoreService(kubernetesClient.Object, null, null,
        //        Given_RuntimeDataRepository_WithResult(false));

        //    var ex = When_Execute_Delete(clientId, data, clientDataService, algoService).Result;

        //    Then_Exception_ShouldNotBeNull(ex);
        //}

        #region Private Methods
        private static ManageImageData Given_ManageImageData()
        {
            return fixture.Build<ManageImageData>().Create();
        }

        private static IKubernetesApiClient Given_Correct_KubernetesApiClientMock_WithResult(bool res)
        {
            var result = new Mock<IKubernetesApiClient>();

            result.Setup(client => client.ListPodsByAlgoIdAsync(It.IsAny<string>())).ReturnsAsync(
                new List<Iok8skubernetespkgapiv1Pod>
                {
                    fixture.Build<Iok8skubernetespkgapiv1Pod>().Create()
                });
            result.Setup(client => client.DeleteAsync(It.IsAny<string>(), It.IsAny<Iok8skubernetespkgapiv1Pod>()))
                .ReturnsAsync(res);

            return result.Object;
        }
        private static IAlgoMetaDataRepository Given_MetaDataRepository_Exists(bool exists)
        {
            var result = new Mock<IAlgoMetaDataRepository>();

            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(exists);
            result.Setup(repo => repo.DeleteAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            return result.Object;
        }
        private static IAlgoClientInstanceRepository Given_InstanceDataRepository_Exists(bool exists)
        {
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo => repo.ExistsAlgoInstanceDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(exists);
            result.Setup(repo =>
                    repo.GetAlgoInstanceDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string clientId, string algoId, string instanceId) =>
                {
                    return fixture.Build<AlgoClientInstanceData>()
                    .With(d => d.ClientId, clientId)
                    .With(d => d.AlgoId, algoId)
                    .With(d => d.InstanceId, instanceId)
                    .Create();
                });
            result.Setup(repo => repo.DeleteAlgoInstanceDataAsync(It.IsAny<AlgoClientInstanceData>())).Returns(Task.CompletedTask);

            return result.Object;
        }


        private static IAlgoBlobRepository Given_BlobRepository_WithResult(bool exists)
        {
            var result = new Mock<IAlgoBlobRepository>();

            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).ReturnsAsync(exists);

            return result.Object;
        }

        private static IAlgoStoreClientDataService Given_ClientDataService(
            IAlgoMetaDataRepository metaDataRepository,
            IAlgoClientInstanceRepository clientInstanceRepository,
            IAlgoBlobRepository blobRepository,
            IKubernetesApiReadOnlyClient kubernetesClient)
        {
            var result = new AlgoStoreClientDataService(metaDataRepository, null, blobRepository,
                clientInstanceRepository, null, null, null, kubernetesClient, new LogMock());

            return result;
        }

        private static IAlgoStoreService Given_AlgoStoreService(IKubernetesApiClient kubernetesApiClient,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IAlgoClientInstanceRepository instanceRepository)
        {
            var result = new AlgoStoreService(new LogMock(), algoBlobRepository, algoMetaDataRepository,
                null, null, null, kubernetesApiClient, instanceRepository);
            return result;
        }

        private static async Task<Exception> When_Execute_Delete(
            ManageImageData data,
            IAlgoStoreClientDataService clientDataService,
            IAlgoStoreService algoStoreService)
        {
            Exception res = null;

            try
            {
                var runtimeData = await clientDataService.ValidateCascadeDeleteClientMetadataRequestAsync(data);

                await algoStoreService.DeleteImageAsync(runtimeData);

                await clientDataService.DeleteMetadataAsync(data);
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
