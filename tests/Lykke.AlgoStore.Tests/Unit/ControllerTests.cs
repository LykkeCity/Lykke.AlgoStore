using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.KubernetesClient.Models;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Services.Utils;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.ClientAccount.Client;
using Moq;
using NUnit.Framework;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;

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
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);

            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true).Object,
                instanceRepo,
                Given_BlobRepository_WithResult(true).Object,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_MetaDataNotExists_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);

            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(false).Object,
                instanceRepo,
                Given_BlobRepository_WithResult(true).Object,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_InstanceDataNotExists_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_ReturnNull();

            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true).Object,
                instanceRepo,
                Given_BlobRepository_WithResult(true).Object,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_PodNotFound_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);

            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true).Object,
                instanceRepo,
                Given_BlobRepository_WithResult(true).Object,
                null,
                Given_PublicAlgoRepository_Exists(false), 
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithoutResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_CantDeletePod_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);

            var clientDataService = Given_ClientDataService(
                Given_MetaDataRepository_Exists(true).Object,
                instanceRepo,
                Given_BlobRepository_WithResult(true).Object,
                null,
                Given_PublicAlgoRepository_Exists(false), 
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(false);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_IsPublic_ReturnSuccess()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);
            var metadataRepoMock = Given_MetaDataRepository_Exists(true);
            var blobRepoMock = Given_BlobRepository_WithResult(true);

            var clientDataService = Given_ClientDataService(
                metadataRepoMock.Object,
                instanceRepo,
                blobRepoMock.Object,
                null,
                Given_PublicAlgoRepository_Exists(true),
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldBeNull(ex);
            metadataRepoMock.Verify(repo => repo.DeleteAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            blobRepoMock.Verify(repo => repo.DeleteBlobAsync(It.IsAny<string>()), Times.Never);
        }
        [Test]
        public void DeleteAlgoMetadataTest_HasInstance_ReturnSuccess()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, true);
            var metadataRepoMock = Given_MetaDataRepository_Exists(true);
            var blobRepoMock = Given_BlobRepository_WithResult(true);

            var clientDataService = Given_ClientDataService(
                metadataRepoMock.Object,
                instanceRepo,
                blobRepoMock.Object,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algoService).Result;

            Then_Exception_ShouldBeNull(ex);
            metadataRepoMock.Verify(repo => repo.DeleteAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            blobRepoMock.Verify(repo => repo.DeleteBlobAsync(It.IsAny<string>()), Times.Never);
        }

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
        private static IKubernetesApiClient Given_Correct_KubernetesApiClientMock_WithoutResult(bool res)
        {
            var result = new Mock<IKubernetesApiClient>();

            result.Setup(client => client.ListPodsByAlgoIdAsync(It.IsAny<string>())).ReturnsAsync(
                new List<Iok8skubernetespkgapiv1Pod>());
            result.Setup(client => client.DeleteAsync(It.IsAny<string>(), It.IsAny<Iok8skubernetespkgapiv1Pod>()))
                .ReturnsAsync(res);

            return result.Object;
        }
        private static Mock<IAlgoMetaDataRepository> Given_MetaDataRepository_Exists(bool exists)
        {
            var result = new Mock<IAlgoMetaDataRepository>();

            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(exists);
            result.Setup(repo => repo.DeleteAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            return result;
        }
        private static IAlgoClientInstanceRepository Given_InstanceDataRepository_Exists(bool exists, bool metadataHasInstance)
        {
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo => repo.ExistsAlgoInstanceDataWithAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(exists);
            result.Setup(repo => repo.ExistsAlgoInstanceDataWithClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(exists);
            result.Setup(repo =>
                    repo.GetAlgoInstanceDataByAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string algoId, string instanceId) =>
                {
                    return fixture.Build<AlgoClientInstanceData>()
                    .With(d => d.AlgoId, algoId)
                    .With(d => d.InstanceId, instanceId)
                    .Create();
                });
            result.Setup(repo =>
                    repo.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string clientId, string instanceId) =>
                {
                    return fixture.Build<AlgoClientInstanceData>()
                        .With(d => d.ClientId, clientId)
                        .With(d => d.InstanceId, instanceId)
                        .Create();
                });
            result.Setup(repo => repo.DeleteAlgoInstanceDataAsync(It.IsAny<AlgoClientInstanceData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.HasInstanceData(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(metadataHasInstance);

            return result.Object;
        }
        private static IAlgoClientInstanceRepository Given_InstanceDataRepository_ReturnNull()
        {
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo => repo.ExistsAlgoInstanceDataWithAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            result.Setup(repo => repo.ExistsAlgoInstanceDataWithClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            result.Setup(repo =>
                    repo.GetAlgoInstanceDataByAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((AlgoClientInstanceData)null);
            result.Setup(repo =>
                    repo.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((AlgoClientInstanceData)null);
            result.Setup(repo => repo.DeleteAlgoInstanceDataAsync(It.IsAny<AlgoClientInstanceData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.HasInstanceData(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            return result.Object;
        }
        private static IPublicAlgosRepository Given_PublicAlgoRepository_Exists(bool exists)
        {
            var result = new Mock<IPublicAlgosRepository>();

            result.Setup(repo => repo.ExistsPublicAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(exists);

            return result.Object;
        }

        private static Mock<IAlgoBlobRepository> Given_BlobRepository_WithResult(bool exists)
        {
            var result = new Mock<IAlgoBlobRepository>();

            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).ReturnsAsync(exists);

            return result;
        }

        private static IAlgoStoreClientDataService Given_ClientDataService(
            IAlgoMetaDataRepository metaDataRepository,
            IAlgoClientInstanceRepository clientInstanceRepository,
            IAlgoBlobRepository blobRepository,
            IKubernetesApiReadOnlyClient kubernetesClient,
            IPublicAlgosRepository publicAlgosRepository,
            IClientAccountClient clientAccountClient,
            AssetsValidator assetsValidator)
        {
            var result = new AlgoStoreClientDataService(metaDataRepository, null, blobRepository,
                clientInstanceRepository, null, publicAlgosRepository, null, null, kubernetesClient, clientAccountClient, assetsValidator, new LogMock());

            return result;
        }

        private static IAlgoStoreService Given_AlgoStoreService(IKubernetesApiClient kubernetesApiClient,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IAlgoClientInstanceRepository instanceRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IStatisticsRepository statisticsRepository,
            IUserLogRepository userLogRepository)
        {
            var result = new AlgoStoreService(new LogMock(), algoBlobRepository, algoMetaDataRepository,
                null, null, kubernetesApiClient, instanceRepository, publicAlgosRepository, statisticsRepository, userLogRepository);
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
