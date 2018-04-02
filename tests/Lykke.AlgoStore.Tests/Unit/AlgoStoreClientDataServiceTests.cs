using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using JetBrains.Annotations;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.KubernetesClient.Models;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Services.Utils;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.Models;
using Lykke.Service.PersonalData.Client.Models;
using Lykke.Service.PersonalData.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Rest;
using Moq;
using NUnit.Framework;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoStoreClientDataServiceTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private const string AlgoClientId = "086ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";

        private const int AssetAccuracy = 3;
        private const int MinVolume = 1;

        private const string TradedAsset = "BTC";
        private const string QuotingAsset = "USD";
        private const string AssetPair = "BTCUSD";

        private static readonly string BlobKey = "TestKey";
        private static readonly string AlogId = "AlgoId123";
        private static readonly Random rnd = new Random();
        private static readonly byte[] BlobBytes = Encoding.Unicode.GetBytes(BlobKey);

        #region Data Generation
        public static IEnumerable<Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses>> StatusesData
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
            var service = Given_AlgoStoreClientDataService(algoClientMetaDataRepo, blobRepository, null, null, null, null, null, null, null, null, null, null, null);
            var uploadBinaryModel = Given_UploadAlgoBinaryData_Model();
            When_Invoke_SaveAlgoAsBinary(service, uploadBinaryModel);
            ThenAlgo_Binary_ShouldExist(uploadBinaryModel.AlgoId, blobRepository);
        }

        [TestCaseSource(nameof(StatusesData))]
        public void GetClientMetadata_Returns_Data(Tuple<ClientAlgoRuntimeStatuses, AlgoRuntimeStatuses> statuses)
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var deploymentClient = Given_Correct_DeploymentApiClientMock(statuses.Item1);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, null, null, null, null,null, null, deploymentClient, null, null, null);
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
            Then_Data_ShouldBe_WithCorrectStatus(data, statuses.Item1);
        }
        [Test]
        public void GetClientMetadata_Returns_DataWithStatus()
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var runtimeRepo = Given_Correct_AlgoRuntimeDataRepositoryMock();
            var deploymentClient = Given_Correct_DeploymentApiClientMock(ClientAlgoRuntimeStatuses.Created);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, runtimeRepo, null, null, null, null, null, null, deploymentClient, null, null, null);
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAllAlgos_Returns_Ok()
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var ratingsRepo = Given_Correct_AlgoRatingsRepositoryMock();
            var publicAlgosRepository = Given_Correct_PublicAlgosRepositoryMock();

            var service = Given_AlgoStoreClientDataService(repo, null, null, null, ratingsRepo, publicAlgosRepository, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAllAlgos(service, out Exception exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
            Then_Algos_ShouldHave_Ratings(data);
            Then_Algos_ShouldHave_UsersCount(data);
        }

        [Test, Explicit("Can fail due to missing ratings for all algos in the DB")]
        public void GetAlgoRating_Returns_Ok()
        {
            var repo = Given_Correct_AlgoRatingsRepositoryMock();
            var service = Given_AlgoStoreClientDataService(null, null, null, null, repo, null, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAlgoRating(service, out Exception exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Result_ShouldNotBe_Empty(data);
            Then_Algos_ShouldHave_Ratings(data);
            Then_Algos_ShouldHave_UsersCount(data);
        }

        [Test]
        public void GetAlgoRatingByClient_Returns_Ok()
        {
            var repo = Given_Correct_AlgoRatingsRepositoryMock();
            var service = Given_AlgoStoreClientDataService(null, null, null, null, repo, null, null, null, null, null, null, null, null);
            var allAlgos = When_Invoke_GetAllAlgos(service, out Exception ex);
            var data = When_Invoke_GetAlgoRatingByClient(service, AlgoStoreClientDataServiceTests.AlogId, ClientId, out Exception exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Result_ShouldNotBe_Empty(data);
            Then_Algos_ShouldHave_Ratings(data);
            Then_Algos_ShouldHave_UsersCount(data);
        }

        [Test]
        public void GetAlgoRatingsByWrongClientId_Returns_EmptyArray()
        {
            var repo = Given_Correct_AlgoRatingsRepositoryMock();
            var service = Given_AlgoStoreClientDataService(null, null, null, null, repo, null, null, null, null, null, null, null, null);
            var allAlgos = When_Invoke_GetAllAlgos(service, out Exception ex);
            var data = When_Invoke_GetAlgoRatingByClient(service, AlgoStoreClientDataServiceTests.AlogId, Guid.NewGuid().ToString(), out Exception exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Result_ShouldNotBe_Empty(data);
            Then_Algos_ShouldHave_Ratings(data);
            Then_Algos_ShouldHave_UsersCount(data);
        }

        [Test, Explicit("Modify data in Table Storage")]
        public void SaveAlgoRating_Returns_Ok()
        {
            var repo = Given_Correct_AlgoRatingsRepositoryMock();
            var publicRepo = Given_Correct_PublicAlgosRepositoryMock();
            var metadataRepo = Given_Correct_AlgoMetaDataRepositoryMock();
            var service = Given_AlgoStoreClientDataService(metadataRepo, null, null, null, repo, publicRepo, null, null, null, null, null, null, null);
            var allAlgos = When_Invoke_GetAllAlgos(service, out Exception ex);

            var randIndex = rnd.Next(0, allAlgos.Count);
            var algoId = allAlgos[randIndex].AlgoId;
            var clientId = allAlgos[randIndex].ClientId;

            var ratingData = new AlgoRatingData
            {
                AlgoId = algoId,
                ClientId = clientId,
                Rating = rnd.NextDouble() * (6 - 1) + 1
            };

            When_Invoke_SaveAlgoRating(service, ratingData, out Exception exception);

            var result = When_Invoke_GetAlgoRatingByClient(service, algoId, clientId, out Exception exc);

            Then_Exception_ShouldBe_Null(exception);
            Then_Result_ShouldNotBe_Empty(result);
            Then_Algos_ShouldHave_Ratings(result);
            Then_Algos_ShouldHave_UsersCount(result);
        }

        [Test]
        public void GetClientMetadata_Throws_Exception()
        {
            var repo = Given_Error_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var deploymentClient = Given_Correct_DeploymentApiClientMock(ClientAlgoRuntimeStatuses.Created);
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null, null, null, null, null, null, deploymentClient, null, null, null);
            var data = When_Invoke_GetClientMetadata(service, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAlgoMetaDataInformation_Returns_Data()
        {
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var ratingsRepo = Given_Correct_AlgoRatingsRepositoryMock();

            var clientId = Guid.NewGuid().ToString();

            var clientAccountService = Given_Customized_ClientAccountServiceMock(clientId);

            var service = Given_AlgoStoreClientDataService(repo, null, null, null, ratingsRepo, null, null, null, clientAccountService, null, null, null, null);
            var data = When_Invoke_GetAlgoMetaDataInformation(service, clientId, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAlgoMetaDataInformation_NoAlgo_Returns_Data()
        {
            var repo = Given_NoAlgoInfo_AlgoMetaDataRepositoryMock();
            var ratingsRepo = Given_Correct_AlgoRatingsRepositoryMock();

            var clientId = Guid.NewGuid().ToString();

            var clientAccountService = Given_Customized_ClientAccountServiceMock(clientId);

            var service = Given_AlgoStoreClientDataService(repo, null, null, null, ratingsRepo, null, null, null, clientAccountService, null, null, null, null);
            var data = When_Invoke_GetAlgoMetaDataInformation(service, clientId, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAlgoMetaDataInformation_Throws_Exception()
        {
            var service = Given_AlgoStoreClientDataService(null, null, null, null, null, null, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAlgoMetaDataInformation(service, null, null, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAlgoMetaDataInformation_NoRepoInstases_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();

            var service = Given_AlgoStoreClientDataService(null, null, null, null, null, null, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAlgoMetaDataInformation(service, clientId, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void SaveClientMetadata_Returns_Ok()
        {
            var clientId = Guid.NewGuid().ToString();
            var data = Given_AlgoClientMetaData();
            var repo = Given_Correct_AlgoMetaDataRepositoryMock();
            var blobRepo = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null, null, null, null, null, null, null, null, null, null);
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
            var service = Given_AlgoStoreClientDataService(repo, blobRepo, null, null, null, null, null, null, null, null, null, null, null);
            When_Invoke_SaveClientMetadata(service, clientId, data, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void GetAllAlgoInstanceDataAsync_Returns_Ok()
        {
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock_ByClientId();
            var service = Given_AlgoStoreClientDataService(null, null, null, repo, null, null, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAllAlgoInstanceDataAsync_Returns_NotFound()
        {
            var repo = Given_Empty_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoStoreClientDataService(null, null, null, repo, null, null, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAllAlgoInstanceDataAsync_Throws_Exception()
        {
            var repo = Given_Error_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoStoreClientDataService(null, null, null, repo, null, null, null, null, null, null, null, null, null);
            When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void GetAlgoInstanceDataAsync_Returns_Ok()
        {
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoStoreClientDataService(null, null, null, repo, null, null, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAlgoInstanceDataAsync_Returns_NotFound()
        {
            var repo = Given_Empty_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoStoreClientDataService(null, null, null, repo, null, null, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAlgoInstanceDataAsync_Throws_Exception()
        {
            var repo = Given_Error_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoStoreClientDataService(null, null, null, repo, null,null, null, null, null, null, null, null, null);
            When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Ok()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var walletBalanceService = Given_Customized_WalletBalanceServiceMock();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, walletBalanceService);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(result);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_WalletNotFound()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, null);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, null);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }


        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_WalletAlreadyUsed()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_WalletExist_Mock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, null, assetService, null, null,
                clientAccountService, assetsValidator, null);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_AlgoNotFound()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(false);
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, null);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_AlgoNotPublic()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(false);
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var publicAlgosRepository = Given_NotPublic_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, null);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_VolumeAccuracy()
        {
            var data = Given_AlgoClientInstanceData(5.00003);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(true);
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, null);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_AssetPairNotExists()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.NotFound);
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, null);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_NoDataSaved()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_Empty_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(true);
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, null);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Throws_Exception()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_Error_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, null);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_BaseAssetNotFound()
        {
            var data = Given_AlgoClientInstanceData(1);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var repoMetadata = Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceMock(data, HttpStatusCode.OK);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var walletBalanceService = Given_Customized_WalletBalanceServiceMock();
            var service = Given_AlgoStoreClientDataService(repoMetadata, null, null, repo, null, publicAlgosRepository, statisticsRepo, assetService, null, null,
                clientAccountService, assetsValidator, walletBalanceService);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(result);
        }

        #region Private Methods

        private static void ThenAlgo_Binary_ShouldExist(string algoId, IAlgoBlobRepository blobRepository)
        {
            var blobExists = blobRepository.BlobExistsAsync(algoId).Result;
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
            service.SaveAlgoAsBinaryAsync(ClientId, model).Wait();
        }

        private static AlgoStoreClientDataService Given_AlgoStoreClientDataService(
            IAlgoMetaDataRepository repo,
            IAlgoBlobRepository blobRepo,
            IAlgoRuntimeDataRepository runtimeDataRepository,
            IAlgoClientInstanceRepository algoInstanceRepository,
            IAlgoRatingsRepository algoRatingsRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IStatisticsRepository statisticsRepository,
            IAssetsService assetsService,
            IPersonalDataService personalDataService,
            IKubernetesApiReadOnlyClient deploymentClient,
            IClientAccountClient clientAccountClient,
            AssetsValidator assetsValidator,
            IWalletBalanceService walletBalanceService)
        {
            return new AlgoStoreClientDataService(repo, runtimeDataRepository, blobRepo, algoInstanceRepository,
                algoRatingsRepository, publicAlgosRepository, statisticsRepository, assetsService, personalDataService, deploymentClient, clientAccountClient, assetsValidator, walletBalanceService, new LogMock());
        }

        private static AlgoClientMetaData When_Invoke_GetClientMetadata(AlgoStoreClientDataService service, string clientId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetClientMetadataAsync(clientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }


        private static AlgoClientMetaDataInformation When_Invoke_GetAlgoMetaDataInformation(AlgoStoreClientDataService service, string clientId, string algoId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAlgoMetaDataInformationAsync(clientId, algoId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static List<AlgoRatingMetaData> When_Invoke_GetAllAlgos(AlgoStoreClientDataService service, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAllAlgosWithRatingAsync().Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static AlgoRatingData When_Invoke_GetAlgoRating(AlgoStoreClientDataService service, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAlgoRatingAsync(AlogId, ClientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static AlgoRatingData When_Invoke_GetAlgoRatingByClient(AlgoStoreClientDataService service, string algoId, string clientId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAlgoRatingForClientAsync(algoId, clientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static AlgoRatingData When_Invoke_SaveAlgoRating(AlgoStoreClientDataService service, AlgoRatingData data, out Exception exception)
        {
            exception = null;
            try
            {
                return service.SaveAlgoRatingAsync(data).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static void When_Invoke_SaveClientMetadata(AlgoStoreClientDataService service, string clientId, AlgoMetaData data, out Exception exception)
        {
            exception = null;
            try
            {
                service.SaveClientMetadataAsync(clientId, string.Empty, data).Wait();
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

        private static void Then_Data_ShouldBe_WithCorrectStatus(AlgoClientMetaData data, ClientAlgoRuntimeStatuses status)
        {
            Assert.AreEqual(data.AlgoMetaData[0].Status, status.ToUpperText());
        }

        private static void Then_Algos_ShouldHave_Ratings(List<AlgoRatingMetaData> data)
        {
            data.ForEach(metadata =>
            {
                Assert.NotNull(metadata.Rating);
            });
        }

        private static void Then_Algos_ShouldHave_Ratings(List<AlgoRatingData> data)
        {
            data.ForEach(algo =>
            {
                Assert.NotNull(algo.Rating);
            });
        }

        private static void Then_Algos_ShouldHave_Ratings(AlgoRatingData data)
        {
            Assert.NotNull(data.Rating);
        }

        private static void Then_Algos_ShouldHave_UsersCount(List<AlgoRatingMetaData> data)
        {
            data.ForEach(metadata =>
            {
                Assert.NotNull(metadata.RatedUsersCount);
            });
        }

        private static void Then_Algos_ShouldHave_UsersCount(List<AlgoRatingData> data)
        {
            data.ForEach(algo =>
            {
                Assert.NotNull(algo.RatedUsersCount);
            });
        }

        private static void Then_Algos_ShouldHave_UsersCount(AlgoRatingData data)
        {

            Assert.NotNull(data.RatedUsersCount);

        }

        private static void Then_Data_ShouldBe_Empty(AlgoClientMetaData data)
        {
            Assert.Null(data);
        }
        private static void Then_Exception_ShouldBe_Null(Exception exception)
        {
            Assert.Null(exception);
        }

        private static void Then_Result_ShouldNotBe_Empty(List<AlgoRatingData> data)
        {
            Assert.IsNotNull(data);
            Assert.NotZero(data.Count);
        }

        private static void Then_Result_ShouldNotBe_Empty(AlgoRatingData data)
        {
            Assert.IsNotNull(data);
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
            result.Setup(repo => repo.GetAllAlgos())
                .Returns(() =>
                {
                    return Task.FromResult(fixture.Build<AlgoClientMetaData>().With(algo => algo.AlgoMetaData, new List<AlgoMetaData> {
                    new AlgoMetaData()
                    {
                        AlgoId = Guid.NewGuid().ToString()
                    }
                }).Create());
                });
            result.Setup(repo => repo.DeleteAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.GetAllClientAlgoMetaDataAsync(It.IsAny<string>()))
                .Returns((string clientId) => { return Task.FromResult(fixture.Build<AlgoClientMetaData>().With(a => a.ClientId, clientId).Create()); });
            result.Setup(repo => repo.GetAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientid, string id) =>
                {
                    var res = new AlgoClientMetaData
                    {
                        ClientId = clientid,
                        AlgoMetaData = new List<AlgoMetaData>()
                    };
                    var data = fixture.Build<AlgoMetaData>()
                        .With(a => a.AlgoId, id)
                        .Create();
                    res.AlgoMetaData.Add(data);

                    return Task.FromResult(res);
                });
            result.Setup(repo => repo.SaveAlgoMetaDataAsync(It.IsAny<AlgoClientMetaData>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.DeleteAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            result.Setup(repo => repo.GetAlgoMetaDataInformationAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientid, string algoId) =>
                {
                    var res = new AlgoClientMetaDataInformation
                    {
                        AlgoId = algoId,
                        AlgoMetaDataInformation = new AlgoMetaDataInformation()
                        {
                            Parameters = new List<AlgoMetaDataParameter>(),
                            Functions = new List<AlgoMetaDataFunction>()
                        }
                    };
                    return Task.FromResult(res);
                });
            return result.Object;
        }

        private static IAlgoMetaDataRepository Given_NoAlgoInfo_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoMetaDataRepository>();
            result.Setup(repo => repo.GetAlgoMetaDataInformationAsync(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns((string clientid, string algoId) =>
                 {
                     return Task.FromResult<AlgoClientMetaDataInformation>(null);
                 });

            return result.Object;

        }

        private static IAlgoRatingsRepository Given_Correct_AlgoRatingsRepositoryMock()
        {
            var result = new Mock<IAlgoRatingsRepository>();
            result.Setup(repo => repo.GetAlgoRatingForClientAsync(It.IsAny<string>(), It.IsAny<string>())).Returns((string clientId, string algoId) =>
            {
                return Task.FromResult(new AlgoRatingData()
                {
                    Rating = Math.Round(rnd.NextDouble() * (6 - 1) + 1, 2),
                    RatedUsersCount = rnd.Next(0, 201)
                });
            });

            return result.Object;
        }

        private static IPublicAlgosRepository Given_Correct_PublicAlgosRepositoryMock()
        {
            var result = new Mock<IPublicAlgosRepository>();
            result.Setup(repo => repo.GetAllPublicAlgosAsync()).Returns(() =>
            {
                return Task.FromResult(new List<PublicAlgoData>()
                {
                    new PublicAlgoData()
                    {
                        ClientId = Guid.NewGuid().ToString(),
                        AlgoId = Guid.NewGuid().ToString()
                    }
                });
            });

            return result.Object;
        }

        private static IPublicAlgosRepository Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock()
        {
            var result = new Mock<IPublicAlgosRepository>();
            result.Setup(repo => repo.ExistsPublicAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
            {
                return Task.FromResult(true);
            });

            return result.Object;
        }

        private static IPublicAlgosRepository Given_NotPublic_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock()
        {
            var result = new Mock<IPublicAlgosRepository>();
            result.Setup(repo => repo.ExistsPublicAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
            {
                return Task.FromResult(false);
            });

            return result.Object;
        }

        private static IAlgoMetaDataRepository Given_Correct_AlgoMetaDataRepositoryMock_With_Exists(bool exists)
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoMetaDataRepository>(); ;
            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(exists));

            return result.Object;
        }

        private static IAlgoBlobRepository Given_Correct_AlgoBlobRepositoryMock()
        {
            var result = new Mock<IAlgoBlobRepository>();
            result.Setup(repo => repo.BlobExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            result.Setup(repo => repo.SaveBlobAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.DeleteBlobAsync(It.IsAny<string>())).Returns(Task.FromResult(new byte[0]));

            return result.Object;
        }

        private static IAlgoRuntimeDataRepository Given_Correct_AlgoRuntimeDataRepositoryMock()
        {
            var result = new Mock<IAlgoRuntimeDataRepository>();
            result.Setup(repo => repo.GetAlgoRuntimeDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientId, string algoId) =>
                {
                    var res = new AlgoClientRuntimeData
                    {
                        AlgoId = algoId,
                        ClientId = clientId
                    };
                    //res.ImageId = 1;

                    return Task.FromResult(res);
                });

            return result.Object;
        }


        private static IAlgoMetaDataRepository Given_Error_AlgoMetaDataRepositoryMock()
        {
            var result = new Mock<IAlgoMetaDataRepository>();
            result.Setup(repo => repo.DeleteAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Delete"));
            result.Setup(repo => repo.GetAllClientAlgoMetaDataAsync(It.IsAny<string>())).ThrowsAsync(new Exception("GetAll"));
            result.Setup(repo => repo.GetAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.SaveAlgoMetaDataAsync(It.IsAny<AlgoClientMetaData>())).ThrowsAsync(new Exception("Save"));
            result.Setup(repo => repo.ExistsAlgoMetaDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }

        private static AlgoMetaData Given_AlgoClientMetaData()
        {
            var fixture = new Fixture();
            return fixture.Build<AlgoMetaData>().Create();
        }

        private static IKubernetesApiReadOnlyClient Given_Correct_DeploymentApiClientMock(ClientAlgoRuntimeStatuses status)
        {
            var result = new Mock<IKubernetesApiReadOnlyClient>();

            result.Setup(repo => repo.ListPodsByAlgoIdAsync(It.IsAny<string>())).ReturnsAsync(new List<Iok8skubernetespkgapiv1Pod>
            {
                new Fixture().Build<Iok8skubernetespkgapiv1Pod>()
                .With(kub => kub.Status, new Iok8skubernetespkgapiv1PodStatus {Phase = status.ToUpperText()})
                .Create()
            });

            return result.Object;
        }

        private static IAlgoClientInstanceRepository Given_Correct_AlgoClientInstanceRepositoryMock_ByClientId()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo => repo.GetAllAlgoInstancesByAlgoIdAndClienIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string algoId, string clientId) =>
                {
                    return Task.FromResult(new List<AlgoClientInstanceData>
                    {
                        fixture.Build<AlgoClientInstanceData>().With(b => b.AlgoId, algoId)
                        .With(b => b.ClientId, ClientId).Create()
                    }.AsEnumerable());
                });

            return result.Object;
        }

        private static IAlgoClientInstanceRepository Given_Correct_AlgoClientInstanceRepositoryMock()
        {
            var result = Correct_AlgoClientInstanceRepositoryMock();

            return result.Object;
        }

        private static IAlgoClientInstanceRepository Given_WalletExist_Mock()
        {
            var result = Correct_AlgoClientInstanceRepositoryMock();

            var fixture = new Fixture();


            result.Setup(repo => repo.GetAllAlgoInstancesByWalletIdAsync(It.IsAny<string>()))
               .Returns((string walletId) =>
               {
                   return Task.FromResult(new List<AlgoClientInstanceData>
                   {
                       fixture.Build<AlgoClientInstanceData>().With(b => b.WalletId, walletId).Create()
                   }.AsEnumerable());
               });


            return result.Object;
        }

        private static Mock<IAlgoClientInstanceRepository> Correct_AlgoClientInstanceRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo => repo.GetAllAlgoInstancesByAlgoAsync(It.IsAny<string>()))
                .Returns((string algoId, string clientId) =>
                {
                    return Task.FromResult(new List<AlgoClientInstanceData>
                    {
                        fixture.Build<AlgoClientInstanceData>().With(b => b.AlgoId, algoId).Create()
                    });
                });

            result.Setup(repo => repo.GetAllAlgoInstancesByClientAsync(It.IsAny<string>()))
                .Returns((string clientId) =>
                {
                    return Task.FromResult(new List<AlgoClientInstanceData>
                    {
                        fixture.Build<AlgoClientInstanceData>().With(a => a.ClientId, clientId).Create()
                    });
                });

            result.Setup(repo => repo.GetAlgoInstanceDataByAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string algoId, string id) =>
                {
                    return Task.FromResult(fixture.Build<AlgoClientInstanceData>()
                        .With(b => b.AlgoId, algoId)
                        .With(b => b.InstanceId, id)
                        .Create());
                });

            result.Setup(repo => repo.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientId, string id) =>
                {
                    return Task.FromResult(fixture.Build<AlgoClientInstanceData>()
                        .With(a => a.ClientId, clientId)
                        .With(b => b.InstanceId, id)
                        .Create());
                });
            result.Setup(repo => repo.SaveAlgoInstanceDataAsync(It.IsAny<AlgoClientInstanceData>())).Returns(Task.CompletedTask);

            return result;
        }


        private static IAlgoClientInstanceRepository Given_Empty_AlgoClientInstanceRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo => repo.GetAllAlgoInstancesByAlgoAsync(It.IsAny<string>()))
                .Returns((string algoId) => Task.FromResult(new List<AlgoClientInstanceData>()));

            result.Setup(repo => repo.GetAllAlgoInstancesByClientAsync(It.IsAny<string>()))
                .Returns((string clientId) => Task.FromResult(new List<AlgoClientInstanceData>()));

            result.Setup(repo => repo.GetAlgoInstanceDataByAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult((AlgoClientInstanceData)null));

            result.Setup(repo => repo.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult((AlgoClientInstanceData)null));

            return result.Object;
        }

        private static IAlgoClientInstanceRepository Given_Error_AlgoClientInstanceRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoClientInstanceRepository>();

            result.Setup(repo => repo.GetAllAlgoInstancesByAlgoAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("GetAllAlgoInstanceDataAsync"));

            result.Setup(repo => repo.GetAllAlgoInstancesByClientAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("GetAllAlgoInstanceDataAsync"));

            result.Setup(repo =>
                    repo.GetAlgoInstanceDataByAlgoIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("GetAlgoInstanceDataAsync"));

            result.Setup(repo =>
                    repo.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("GetAlgoInstanceDataAsync"));

            result.Setup(repo =>
                 repo.GetAllAlgoInstancesByAlgoIdAndClienIdAsync(It.IsAny<string>(), It.IsAny<string>()))
             .ThrowsAsync(new Exception("GetAllAlgoInstancesByAlgoIdAndClienIdAsync"));

            result.Setup(repo =>
                 repo.GetAllAlgoInstancesByWalletIdAsync(It.IsAny<string>()))
             .ThrowsAsync(new Exception("GetAllAlgoInstancesByWalletIdAsync"));

            result.Setup(repo => repo.SaveAlgoInstanceDataAsync(It.IsAny<AlgoClientInstanceData>())).ThrowsAsync(new Exception("SaveAlgoInstanceDataAsync"));

            return result.Object;
        }

        private static IStatisticsRepository Given_Correct_StatisticsRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.CreateOrUpdateSummaryAsync(It.IsAny<StatisticsSummary>())).Returns(Task.CompletedTask);

            result.Setup(repo => repo.GetSummaryAsync(It.IsAny<string>())).Returns(
                () => Task.FromResult(new StatisticsSummary()));

            return result.Object;
        }

        private static IAssetsService Given_Customized_AssetServiceMock(AlgoClientInstanceData data, HttpStatusCode statusCode)
        {
            var fixture = new Fixture();
            var result = new Mock<IAssetsService>();

            result.Setup(service => service.AssetPairGetWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpOperationResponse<AssetPair>
                {
                    Body = fixture.Build<AssetPair>()
                    .With(pair => pair.QuotingAssetId, data.TradedAsset)
                    .With(pair => pair.Id, data.AssetPair)
                    .With(pair => pair.Accuracy, AssetAccuracy)
                    .With(pair => pair.IsDisabled, false)
                    .With(pair => pair.MinVolume, MinVolume)
                    .With(pair => pair.MinInvertedVolume, MinVolume)
                    .Create(),
                    Response = new HttpResponseMessage(statusCode)
                });

            result.Setup(service => service.AssetGetWithHttpMessagesAsync(It.IsAny<string>(),
                    It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpOperationResponse<Asset>
                {
                    Body = fixture.Build<Asset>()
                    .With(asset => asset.Name, data.TradedAsset)
                    .With(asset => asset.Id, data.TradedAsset)
                    .With(asset => asset.Accuracy, AssetAccuracy)
                    .With(asset => asset.IsDisabled, false)
                    .Create(),
                    Response = new HttpResponseMessage(statusCode)
                });

            return result.Object;
        }

        private static IPersonalDataService Given_Customized_ClientAccountServiceMock(string clientId)
        {
            var result = new Mock<IPersonalDataService>();

            result.Setup(service => service.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new PersonalDataModel()
                {
                    Id = clientId,
                    Email = "test@m.com",
                    FullName = "Test Test"
                });

            return result.Object;
        }

        private static IClientAccountClient Given_Customized_ClientAccountClientMock(string clientId,
            [CanBeNull] string walletId)
        {
            var fixture = new Fixture();
            var result = new Mock<IClientAccountClient>();

            result.Setup(service => service.GetWalletsByClientIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<WalletDtoModel>
                    {
                        fixture.Build<WalletDtoModel>()
                            .With(w => w.Id, walletId)
                            .Create()
                    });

            result.Setup(service => service.GetBaseAssetAsync(It.IsAny<string>()))
                .ReturnsAsync(new BaseAssetClientModel
                {
                    BaseAssetId = Guid.NewGuid().ToString()
                });

            return result.Object;
        }

        private static IWalletBalanceService Given_Customized_WalletBalanceServiceMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IWalletBalanceService>();

            result.Setup(service => service.GetTotalWalletBalanceInBaseAssetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AssetPair>()))
                .ReturnsAsync(100);

            result.Setup(service => service.GetWalletBalancesAsync(It.IsAny<string>(), It.IsAny<AssetPair>()))
                .ReturnsAsync(new List<ClientBalanceResponseModel>
                {
                    fixture.Build<ClientBalanceResponseModel>()
                        .With(w => w.AssetId, TradedAsset)
                        .Create(),

                    fixture.Build<ClientBalanceResponseModel>()
                        .With(w => w.AssetId, QuotingAsset)
                        .Create()
                });
            

            return result.Object;
        }

        private static List<AlgoClientInstanceData> When_Invoke_GetAllAlgoInstanceDataAsync(AlgoStoreClientDataService service, string clientId, string algoId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAllAlgoInstanceDataByAlgoIdAndClientIdAsync(new CSharp.AlgoTemplate.Models.Models.BaseAlgoData { ClientId = clientId, AlgoId = algoId }).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }
        private static AlgoClientInstanceData When_Invoke_GetAlgoInstanceDataAsync(AlgoStoreClientDataService service, string clientId, string algoId, string id, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAlgoInstanceDataAsync(new CSharp.AlgoTemplate.Models.Models.BaseAlgoInstance { ClientId = clientId, AlgoId = algoId, InstanceId = id }).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }
        private static AlgoClientInstanceData When_Invoke_SaveAlgoInstanceDataAsync(AlgoStoreClientDataService service, AlgoClientInstanceData data, string algoClientId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.SaveAlgoInstanceDataAsync(data, algoClientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static void Then_Data_ShouldNotBe_Empty(List<AlgoClientInstanceData> data)
        {
            Assert.IsTrue(!data.IsNullOrEmptyCollection());
        }
        private static void Then_Data_ShouldNotBe_Empty(List<AlgoRatingMetaData> data)
        {
            Assert.IsTrue(!data.IsNullOrEmptyCollection());
        }
        private static void Then_Data_ShouldBe_Empty(List<AlgoClientInstanceData> data)
        {
            Assert.IsTrue(data.IsNullOrEmptyCollection());
        }

        private static void Then_Data_ShouldNotBe_Empty(AlgoClientInstanceData data)
        {
            Assert.NotNull(data);
        }

        private static void Then_Data_ShouldNotBe_Empty(AlgoClientMetaDataInformation data)
        {
            Assert.NotNull(data);
        }

        private static void Then_Data_ShouldBe_Empty(AlgoClientMetaDataInformation data)
        {
            Assert.Null(data);
        }

        private static AlgoClientInstanceData Given_AlgoClientInstanceData(double volume)
        {
            var fixture = new Fixture();
            return fixture.Build<AlgoClientInstanceData>()
                .With(a => a.Volume, volume)
                .With(a => a.TradedAsset, TradedAsset)
                .With(a => a.AssetPair, AssetPair)
                .Create();
        }
        #endregion
    }
}
