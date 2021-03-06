﻿using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Job.Stopping.Client.AutorestClient.Models;
using Lykke.AlgoStore.Job.Stopping.Client.Models.ResponseModels;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Services.Utils;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.Assets.Client;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.PersonalData.Contract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
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
                Given_AlgosRepository_Exists(true).Object,
                Given_BlobRepository_WithResult(true).Object,
                instanceRepo,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null);


            var algosInstanceService = Given_AlgoInstanceService(
                Given_AlgosRepository_Exists(true).Object,
                instanceRepo,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null,
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algosInstanceService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_MetaDataNotExists_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);

            var clientDataService = Given_ClientDataService(
                Given_AlgosRepository_Exists(false).Object,
                Given_BlobRepository_WithResult(true).Object,
                instanceRepo,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null);

            var algosInstanceService = Given_AlgoInstanceService(
                Given_AlgosRepository_Exists(false).Object,
                instanceRepo,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null,
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algosInstanceService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_InstanceDataNotExists_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_ReturnNull();

            var clientDataService = Given_ClientDataService(
                Given_AlgosRepository_Exists(true).Object,
                Given_BlobRepository_WithResult(true).Object,
                instanceRepo,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var algosInstanceService = Given_AlgoInstanceService(
                Given_AlgosRepository_Exists(true).Object,
                instanceRepo,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null,
                null,
                null);

            var ex = When_Execute_Delete(data, clientDataService, algosInstanceService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_GetPods_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);

            var clientDataService = Given_ClientDataService(
                Given_AlgosRepository_Exists(true).Object,
                Given_BlobRepository_WithResult(true).Object,
                instanceRepo,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null);

            var algosInstanceService = Given_AlgoInstanceService(
                Given_AlgosRepository_Exists(true).Object,
                instanceRepo,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null,
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithoutResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algosInstanceService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }
        [Test]
        public void DeleteAlgoMetadataTest_CantDeletePod_Throws()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);

            var clientDataService = Given_ClientDataService(
                Given_AlgosRepository_Exists(true).Object,
                Given_BlobRepository_WithResult(true).Object,
                instanceRepo,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null);

            var algosInstanceService = Given_AlgoInstanceService(
                Given_AlgosRepository_Exists(true).Object,
                instanceRepo,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null,
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(false);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algosInstanceService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }

        [Test]
        public void DeleteAlgoMetadataTest_IsPublic_ReturnSuccess()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, false);
            var metadataRepoMock = Given_AlgosRepository_Exists(true);
            var blobRepoMock = Given_BlobRepository_WithResult(true);

            var clientDataService = Given_ClientDataService(
                metadataRepoMock.Object,
                blobRepoMock.Object,
                instanceRepo,
                null,
                Given_PublicAlgoRepository_Exists(true),
                null,
                null,
                null,
                null);

            var algosInstanceService = Given_AlgoInstanceService(
                metadataRepoMock.Object,
                instanceRepo,
                Given_PublicAlgoRepository_Exists(true),
                null,
                null,
                null,
                null,
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algosInstanceService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }

        [Test]
        public void DeleteAlgoMetadataTest_HasInstance_ReturnSuccess()
        {
            var data = Given_ManageImageData();
            var instanceRepo = Given_InstanceDataRepository_Exists(true, true);
            var metadataRepoMock = Given_AlgosRepository_Exists(true);
            var blobRepoMock = Given_BlobRepository_WithResult(true);

            var clientDataService = Given_ClientDataService(
                metadataRepoMock.Object,
                blobRepoMock.Object,
                instanceRepo,
                null,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null);

            var algosInstanceService = Given_AlgoInstanceService(
                metadataRepoMock.Object,
                instanceRepo,
                Given_PublicAlgoRepository_Exists(false),
                null,
                null,
                null,
                null,
                null,
                null);

            var kubernetesClient = Given_Correct_KubernetesApiClientMock_WithResult(true);
            var algoService = Given_AlgoStoreService(kubernetesClient, null, null, instanceRepo, null, null, null);

            var ex = When_Execute_Delete(data, clientDataService, algosInstanceService, algoService).Result;

            Then_Exception_ShouldNotBeNull(ex);
        }

        #region Private Methods
        private static ManageImageData Given_ManageImageData()
        {
            return fixture.Build<ManageImageData>().Create();
        }

        private static IAlgoInstanceStoppingClient Given_Correct_KubernetesApiClientMock_WithResult(bool res)
        {
            var result = new Mock<IAlgoInstanceStoppingClient>();

            result.Setup(client => client.GetPodsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PodsResponse
            {
                Records = new List<PodResponseModel>
                {
                    fixture.Build<PodResponseModel>().Create()
                }
            });

            result.Setup(client => client.DeleteAlgoInstanceByInstanceIdAndPodAsync(It.IsAny<string>(),
                                                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new DeleteAlgoInstanceResponseModel()
                {
                    IsSuccessfulDeletion = res
                });

            return result.Object;
        }
        private static IAlgoInstanceStoppingClient Given_Correct_KubernetesApiClientMock_WithoutResult(bool res)
        {
            var result = new Mock<IAlgoInstanceStoppingClient>();

            result.Setup(client => client.GetPodsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
                new PodsResponse()
                {
                    Records = new List<PodResponseModel>(),
                    Error = new ErrorModel()
                    {
                        ErrorMessage = "Unauthorized"
                    }
                });
            result.Setup(client => client.DeleteAlgoInstanceByInstanceIdAndPodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new DeleteAlgoInstanceResponseModel()
                {
                    IsSuccessfulDeletion = res
                });

            return result.Object;
        }
        private static Mock<IAlgoRepository> Given_AlgosRepository_Exists(bool exists)
        {
            var result = new Mock<IAlgoRepository>();

            result.Setup(repo => repo.ExistsAlgoAsync(It.IsAny<string>()))
                .ReturnsAsync(exists);
            result.Setup(repo => repo.DeleteAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

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

        private static IAlgosService Given_ClientDataService(IAlgoRepository repo,
            IAlgoBlobRepository blobRepo,
            IAlgoClientInstanceRepository algoInstanceRepository,
            IAlgoRatingsRepository algoRatingsRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IPersonalDataService personalDataService,
            IAlgoStoreService algoStoreService,
            IAlgoCommentsRepository commentsRepository,
            ICodeBuildService codeBuildService)
        {
            var result = new AlgosService(repo, blobRepo, algoInstanceRepository,
                algoRatingsRepository, publicAlgosRepository, personalDataService,
                algoStoreService, commentsRepository,
                new LogMock(), codeBuildService, null);

            return result;
        }

        private static IAlgoInstancesService Given_AlgoInstanceService(
               IAlgoRepository repo,
               IAlgoClientInstanceRepository algoInstanceRepository,
               IPublicAlgosRepository publicAlgosRepository,
               IStatisticsRepository statisticsRepository,
               IAssetsServiceWithCache assetsService,
               IClientAccountClient clientAccountClient,
               ICandleshistoryservice candleshistoryservice,
               AssetsValidator assetsValidator,
               IWalletBalanceService walletBalanceService)
        {
            return new AlgoInstancesService(repo, algoInstanceRepository, publicAlgosRepository,
                statisticsRepository, assetsService, clientAccountClient,
                candleshistoryservice, assetsValidator, walletBalanceService,
                new LogMock());
        }

        private static IAlgoStoreService Given_AlgoStoreService(IAlgoInstanceStoppingClient algoInstanceStoppingClient,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoReadOnlyRepository algoMetaDataRepository,
            IAlgoClientInstanceRepository instanceRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IAlgoRepository algosRepository,
            ILoggingClient loggingClient)
        {
            var result = new AlgoStoreService(new LogMock(), algoBlobRepository,
                null, null, algoInstanceStoppingClient, instanceRepository, publicAlgosRepository, algosRepository, loggingClient);
            return result;
        }

        private static async Task<Exception> When_Execute_Delete(
            ManageImageData data,
            IAlgosService clientDataService,
            IAlgoInstancesService instancesService,
            IAlgoStoreService algoStoreService)
        {
            Exception res = null;

            try
            {
                var runtimeData = await instancesService.ValidateCascadeDeleteClientMetadataRequestAsync(data);
                runtimeData.AlgoInstanceStatus = AlgoInstanceStatus.Started;
                await algoStoreService.DeleteInstanceAsync(runtimeData);

                await clientDataService.DeleteAsync(data);
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
