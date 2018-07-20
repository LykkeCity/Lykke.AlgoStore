using AutoFixture;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.AlgoStore.Api.Infrastructure;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Job.Stopping.Client.AutorestClient.Models;
using Lykke.AlgoStore.Job.Stopping.Client.Models.ResponseModels;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Services.Utils;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.Models;
using Lykke.Service.PersonalData.Client.Models;
using Lykke.Service.PersonalData.Contract;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoAndAlgoInstanceServiceTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private const string AlgoClientId = "086ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private static readonly string WalletId = Guid.NewGuid().ToString();

        private const int AssetAccuracy = 3;
        private const int MinVolume = 1;

        private const string TradedAsset = "BTC";
        private const string QuotingAsset = "USD";
        private const string AssetPair = "BTCUSD";

        private const string AssetPairKey = "AssetPair";
        private const string TradedAssetKey = "TradedAsset";

        private static readonly string BlobKey = "TestKey";
        private static readonly string AlgoId = "AlgoId123";
        private static readonly Random rnd = new Random();
        private static readonly byte[] BlobBytes = Encoding.Unicode.GetBytes(BlobKey);

        private static readonly DateTime StartFromDate = DateTime.UtcNow;

        [SetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        [Test]
        public void SaveAlgoAsBinary_Test()
        {
            var algoRepo = Given_Correct_AlgoRepositoryMock();
            var blobRepository = Given_Correct_AlgoBlobRepositoryMock();
            var service = Given_AlgosService(algoRepo, blobRepository, null, null, null,
                null, null, null, null, null);
            var uploadBinaryModel = Given_UploadAlgoBinaryData_Model();
            When_Invoke_SaveAlgoAsBinary(service, uploadBinaryModel);
            ThenAlgo_Binary_ShouldExist(uploadBinaryModel.AlgoId, blobRepository);
        }

        [Test]
        public void GetAllAlgos_Returns_Ok()
        {
            var repo = Given_Correct_AlgoRepositoryMock();
            var ratingsRepo = Given_Correct_AlgoRatingsRepositoryMock();
            var publicAlgosRepository = Given_Correct_PublicAlgosRepositoryMock();
            var personalDataervice = Given_Customized_ClientAccountServiceMock(Guid.NewGuid().ToString());

            var service = Given_AlgosService(repo, null, null, ratingsRepo, publicAlgosRepository, personalDataervice,
                null, null, null, null);
            var data = When_Invoke_GetAllAlgos(service, out Exception exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
            Then_Algos_ShouldHave_Ratings(data);
            Then_Algos_ShouldHave_UsersCount(data);
        }

        [Test]
        public void GetAllUserAlgos_Returns_Ok()
        {
            var repo = Given_Correct_AlgoRepositoryMock();
            var personalDataervice = Given_Customized_ClientAccountServiceMock(Guid.NewGuid().ToString());

            var service = Given_AlgosService(repo, null, null, null, null, personalDataervice, null,
                null, null, null);
            var data = When_Invoke_GetAllUserAlgos(service, out Exception exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAllUserAlgos_Returns_Empty()
        {
            var repo = Given_NoUserAlgos_AlgoRepositoryMock();
            var personalDataervice = Given_Customized_ClientAccountServiceMock(Guid.NewGuid().ToString());

            var service = Given_AlgosService(repo, null, null, null, null, personalDataervice, null, null,
                null, null);
            var data = When_Invoke_GetAllUserAlgos(service, out Exception exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test, Explicit("Can fail due to missing ratings for all algos in the DB")]
        public void GetAlgoRating_Returns_Ok()
        {
            var repo = Given_Correct_AlgoRatingsRepositoryMock();
            var service = Given_AlgosService(null, null, null, repo, null, null, null, null, null,
                null);
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
            var service = Given_AlgosService(null, null, null, repo, null, null, null, null, null,
                null);
            var allAlgos = When_Invoke_GetAllAlgos(service, out Exception ex);
            var data = When_Invoke_GetAlgoRatingByClient(service, AlgoAndAlgoInstanceServiceTests.AlgoId, ClientId, out Exception exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Result_ShouldNotBe_Empty(data);
            Then_Algos_ShouldHave_Ratings(data);
            Then_Algos_ShouldHave_UsersCount(data);
        }

        [Test]
        public void GetAlgoRatingsByWrongClientId_Returns_EmptyArray()
        {
            var repo = Given_Correct_AlgoRatingsRepositoryMock();
            var service = Given_AlgosService(null, null, null, repo, null, null, null, null, null,
                null);
            var allAlgos = When_Invoke_GetAllAlgos(service, out Exception ex);
            var data = When_Invoke_GetAlgoRatingByClient(service, AlgoAndAlgoInstanceServiceTests.AlgoId, Guid.NewGuid().ToString(), out Exception exception);

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
            var algoRepo = Given_Correct_AlgoRepositoryMock();
            var service = Given_AlgosService(algoRepo, null, null, repo, publicRepo, null, null, null, null,
                null);
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
        public void GetAlgoInformation_Returns_Data()
        {
            var repo = Given_Correct_AlgoRepositoryMock();
            var ratingsRepo = Given_Correct_AlgoRatingsRepositoryMock();

            var clientId = Guid.NewGuid().ToString();

            var clientAccountService = Given_Customized_ClientAccountServiceMock(clientId);
            var assetService = Given_AssetsServiceWithCache();

            var service = Given_AlgosService(repo, null, null, ratingsRepo, null,
                clientAccountService, null, null, null, assetService);
            var data = When_Invoke_GetAlgoInformation(service, clientId, clientId, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAssets_For_AssetPair_Returns_Data()
        {
            var assetService = Given_AssetsServiceWithCache();

            var clientId = Guid.NewGuid().ToString();

            var service = Given_AlgosService(null, null, null, null, null,
                null, null, null, null, assetService);

            var data = When_Invoke_GetAssetsForAssetPair(service, clientId, AssetPairKey, out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAlgoInformation_NoAlgo_Returns_Data()
        {
            var repo = Given_NoAlgoInfo_AlgoRepositoryMock();
            var ratingsRepo = Given_Correct_AlgoRatingsRepositoryMock();

            var clientId = Guid.NewGuid().ToString();

            var clientAccountService = Given_Customized_ClientAccountServiceMock(clientId);

            var service = Given_AlgosService(repo, null, null, ratingsRepo, null,
                clientAccountService, null, null, null, null);
            var data = When_Invoke_GetAlgoInformation(service, clientId, clientId, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAlgoInformation_Throws_Exception()
        {
            var service = Given_AlgosService(null, null, null, null, null, null, null, null,
                null, null);
            var data = When_Invoke_GetAlgoInformation(service, null, null, null, out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAlgoInformation_NoRepoInstases_Throws_Exception()
        {
            var clientId = Guid.NewGuid().ToString();

            var service = Given_AlgosService(null, null, null, null, null, null, null, null,
                null, null);
            var data = When_Invoke_GetAlgoInformation(service, clientId, clientId, Guid.NewGuid().ToString(), out var exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAllAlgoInstanceDataAsync_Returns_Ok()
        {
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock_ByClientId();
            var service = Given_AlgoInstanceService(null, repo, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAllAlgoInstanceDataAsync_Returns_NotFound()
        {
            var repo = Given_Empty_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoInstanceService(null, repo, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAllAlgoInstanceDataAsync_Throws_Exception()
        {
            var repo = Given_Error_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoInstanceService(null, repo, null, null, null, null, null, null, null);
            When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void GetAlgoInstanceDataAsync_Returns_Ok()
        {
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock();
            var service = Given_AlgoInstanceService(algoRepo, repo, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(data);
        }

        [Test]
        public void GetAlgoInstanceDataAsync_Returns_NotFound()
        {
            var repo = Given_Empty_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoInstanceService(null, repo, null, null, null, null, null, null, null);
            var data = When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldBe_Empty(data);
        }

        [Test]
        public void GetAlgoInstanceDataAsync_Throws_Exception()
        {
            var repo = Given_Error_AlgoClientInstanceRepositoryMock();
            var service = Given_AlgoInstanceService(null, repo, null, null, null, null, null, null, null);
            When_Invoke_GetAllAlgoInstanceDataAsync(service, Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Ok()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var walletBalanceService = Given_Customized_WalletBalanceServiceMock(true);
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, walletBalanceService);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(result);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_User_Has_One_Asset_Returns_Ok()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var walletBalanceService = Given_Customized_WalletBalanceServiceMock(false);
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, walletBalanceService);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(result);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_WalletNotFound()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, null);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, null);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }


        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_WalletAlreadyUsed()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_WalletExist_Mock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                null, assetService, clientAccountService, null, assetsValidator, null);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_AlgoNotFound()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(false);
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, null);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_AlgoNotPublic()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(false);
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var publicAlgosRepository = Given_NotPublic_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, null);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_VolumeAccuracy()
        {
            var data = Given_AlgoClientInstanceData(5.00003, AlgoInstanceType.Live);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, null);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_AssetPairNotExists()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, true);
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, null);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_StartDate_Is_Later_Than_EndDate()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live, false);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var walletBalanceService = Given_Customized_WalletBalanceServiceMock(true);
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, walletBalanceService);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_NoDataSaved()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Empty_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, null);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Throws_Exception()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Error_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, false);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, null);
            When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void SaveAlgoInstanceDataAsync_Returns_Error_BaseAssetNotFound()
        {
            var data = Given_AlgoClientInstanceData(1, AlgoInstanceType.Live);
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var statisticsRepo = Given_Correct_StatisticsRepositoryMock();
            var algoRepo = Given_Correct_AlgoRepositoryMock_With_Exists(true);
            var publicAlgosRepository = Given_Correct_ExistsPublicAlgoAsync_PublicAlgosRepositoryMock();
            var assetService = Given_Customized_AssetServiceWithCacheMock(data, true);
            var clientAccountService = Given_Customized_ClientAccountClientMock(data.ClientId, data.WalletId);
            var assetsValidator = new AssetsValidator();
            var walletBalanceService = Given_Customized_WalletBalanceServiceMock(true);
            var service = Given_AlgoInstanceService(algoRepo, repo, publicAlgosRepository,
                statisticsRepo, assetService, clientAccountService, null, assetsValidator, walletBalanceService);
            var result = When_Invoke_SaveAlgoInstanceDataAsync(service, data, AlgoClientId, out Exception exception);
            Then_Exception_ShouldNotBe_Null(exception);
            Then_Data_ShouldBe_Empty(result);
        }

        [Test]
        public void GetUserInstancesAsync_Returns_OK()
        {
            var clientId = Guid.NewGuid().ToString();
            var repo = Given_Correct_AlgoClientInstanceRepositoryMock();
            var clientAccountService = Given_Customized_ClientAccountClientMock(clientId, WalletId);
            var service = Given_AlgoInstanceService(null, repo, null,
                null, null, clientAccountService, null, null, null);

            var data = When_Invoke_GetUserAlgosAsync(service, clientId);

            Then_Data_ShouldNotBe_Null(data);
            Then_Live_Instances_Have_Wallet(data);
            Then_Demo_Instances_Wallet_IsNull(data);
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
            var model = new UploadAlgoBinaryData { AlgoId = AlgoId, Data = binaryFile.Object };
            return model;
        }

        private static void When_Invoke_SaveAlgoAsBinary(AlgosService service, UploadAlgoBinaryData model)
        {
            service.SaveAlgoAsBinaryAsync(ClientId, model).Wait();
        }

        private List<UserInstanceData> When_Invoke_GetUserAlgosAsync(IAlgoInstancesService service, string clientId)
        {
            return service.GetUserInstancesAsync(clientId).Result;
        }

        private static AlgosService Given_AlgosService(
            IAlgoRepository repo,
            IAlgoBlobRepository blobRepo,
            IAlgoClientInstanceRepository algoInstanceRepository,
            IAlgoRatingsRepository algoRatingsRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IPersonalDataService personalDataService,
            IAlgoStoreService algoStoreService,
            IAlgoCommentsRepository commentsRepository,
            ICodeBuildService codeBuildService,
            IAssetsServiceWithCache assetsService)
        {
            return new AlgosService(repo, blobRepo, algoInstanceRepository,
                algoRatingsRepository, publicAlgosRepository, personalDataService,
                algoStoreService, commentsRepository,
                new LogMock(), codeBuildService, assetsService);
        }

        private static AlgoInstancesService Given_AlgoInstanceService(
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

        private static AlgoDataInformation When_Invoke_GetAlgoInformation(AlgosService service, string clientId, string algoClientId, string algoId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAlgoDataInformationAsync(clientId, algoClientId, algoId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static List<EnumValue> When_Invoke_GetAssetsForAssetPair(AlgosService service, string clientId, string assetPairId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAssetsForAssetPairAsync(assetPairId, clientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static List<AlgoRatingMetaData> When_Invoke_GetAllAlgos(AlgosService service, out Exception exception)
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

        private static List<AlgoData> When_Invoke_GetAllUserAlgos(AlgosService service, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAllUserAlgosAsync(ClientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static AlgoRatingData When_Invoke_GetAlgoRating(AlgosService service, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAlgoRatingAsync(AlgoId, ClientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static AlgoRatingData When_Invoke_GetAlgoRatingByClient(AlgosService service, string algoId, string clientId, out Exception exception)
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

        private static AlgoRatingData When_Invoke_SaveAlgoRating(AlgosService service, AlgoRatingData data, out Exception exception)
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

        private static void Then_Data_ShouldNotBe_Empty(AlgoData data)
        {
            Assert.NotNull(data);
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

        private static void Then_Data_ShouldBe_Empty(AlgoData data)
        {
            Assert.Null(data);
        }

        private static void Then_Data_ShouldBe_Empty(AlgoClientInstanceData data)
        {
            Assert.Null(data);
        }

        private static void Then_Exception_ShouldBe_Null(Exception exception)
        {
            Assert.Null(exception);
        }

        private static void Then_Exception_ShouldNotBe_Null(Exception exception)
        {
            Assert.NotNull(exception);
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

        private static IAlgoRepository Given_Correct_AlgoRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoRepository>();
            result.Setup(repo => repo.GetAllAlgosAsync())
                .Returns(() =>
                {
                    return Task.FromResult(fixture.Build<IEnumerable<AlgoEntity>>().Create() as IEnumerable<IAlgo>);
                });
            result.Setup(repo => repo.DeleteAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.GetAllClientAlgosAsync(It.IsAny<string>()))
                .Returns((string clientId) =>
                {
                    return Task.FromResult(fixture.Build<AlgoEntity>()
.With(a => a.ClientId, clientId)
.CreateMany() as IEnumerable<IAlgo>);
                });
            result.Setup(repo => repo.GetAlgoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientid, string id) =>
                {
                    var res = fixture.Build<AlgoEntity>()
                        .With(a => a.RowKey, id)
                        .With(a => a.ClientId, clientid)
                        .Create();

                    return Task.FromResult(res as IAlgo);
                });
            result.Setup(repo => repo.SaveAlgoAsync(It.IsAny<IAlgo>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.DeleteAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            result.Setup(repo => repo.ExistsAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var metadataParameters = new List<AlgoMetaDataParameter>();
            fixture.Build<AlgoMetaDataParameter>()
                .With(p => p.Key, AssetPairKey)
                .Without(p => p.PredefinedValues)
                .Do(p => metadataParameters.Add(p))
                .Create();

            fixture.Build<AlgoMetaDataParameter>()
                .With(p => p.Key, TradedAssetKey)
                .Without(p => p.PredefinedValues)
                .Do(p => metadataParameters.Add(p))
                .Create();

            result.Setup(repo => repo.GetAlgoDataInformationAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string clientid, string algoId) =>
                {
                    var res = new AlgoDataInformation
                    {
                        AlgoId = algoId,
                        AlgoMetaDataInformation = new AlgoMetaDataInformation()
                        {
                            Parameters = metadataParameters,
                            Functions = new List<AlgoMetaDataFunction>()
                        }
                    };
                    return Task.FromResult(res);
                });
            return result.Object;
        }

        private static IAlgoRepository Given_NoUserAlgos_AlgoRepositoryMock()
        {
            var result = new Mock<IAlgoRepository>();
            result.Setup(repo => repo.GetAllClientAlgosAsync(It.IsAny<string>()))
                .Returns((string clientId) => Task.FromResult<IEnumerable<IAlgo>>(new List<IAlgo>()));

            return result.Object;
        }

        private static IAlgoRepository Given_NoAlgoInfo_AlgoRepositoryMock()
        {
            var result = new Mock<IAlgoRepository>();
            result.Setup(repo => repo.GetAlgoDataInformationAsync(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns((string clientid, string algoId) =>
                 {
                     return Task.FromResult<AlgoDataInformation>(null);
                 });

            result.Setup(repo => repo.ExistsAlgoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

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

        private static IAlgoRepository Given_Correct_AlgoRepositoryMock_With_Exists(bool exists)
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoRepository>(); ;
            result.Setup(repo => repo.ExistsAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(exists));

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


        private static IAlgoRepository Given_Error_AlgoRepositoryMock()
        {
            var result = new Mock<IAlgoRepository>();
            result.Setup(repo => repo.DeleteAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Delete"));
            result.Setup(repo => repo.GetAllClientAlgosAsync(It.IsAny<string>())).ThrowsAsync(new Exception("GetAll"));
            result.Setup(repo => repo.GetAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Get"));
            result.Setup(repo => repo.SaveAlgoAsync(It.IsAny<IAlgo>())).ThrowsAsync(new Exception("Save"));
            result.Setup(repo => repo.ExistsAlgoAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Exists"));

            return result.Object;
        }

        private static AlgoData Given_AlgoData()
        {
            var fixture = new Fixture();
            return fixture.Build<AlgoData>().Create();
        }

        private static IAlgoInstanceStoppingClient Given_Correct_DeploymentApiClientMock(ClientAlgoRuntimeStatuses status)
        {
            var result = new Mock<IAlgoInstanceStoppingClient>();

            result.Setup(c => c.GetPodsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PodsResponse
            {
                Records = new List<PodResponseModel>()
                {
                    new PodResponseModel()
                    {
                      Phase =status.ToUpperText()
                    }
                }
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


            result.Setup(repo => repo.GetAllByWalletIdAndInstanceStatusIsNotStoppedAsync(It.IsAny<string>()))
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
                    var liveInstances = fixture.Build<AlgoClientInstanceData>()
                    .With(a => a.ClientId, clientId)
                    .With(a => a.WalletId, WalletId)
                    .With(a => a.AlgoInstanceType, AlgoInstanceType.Live)
                    .CreateMany().ToList();

                    var demoInstances = fixture.Build<AlgoClientInstanceData>()
                    .With(a => a.ClientId, clientId)
                    .Without(a => a.WalletId)
                    .With(a => a.AlgoInstanceType, AlgoInstanceType.Demo)
                    .CreateMany().ToList();

                    var testInstances = fixture.Build<AlgoClientInstanceData>()
                    .With(a => a.ClientId, clientId)
                    .Without(a => a.WalletId)
                    .With(a => a.AlgoInstanceType, AlgoInstanceType.Test)
                    .CreateMany().ToList();

                    var instances = new List<AlgoClientInstanceData>();

                    instances.AddRange(liveInstances);
                    instances.AddRange(demoInstances);
                    instances.AddRange(testInstances);
                    
                    return Task.FromResult(instances);
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
                 repo.GetAllByWalletIdAndInstanceStatusIsNotStoppedAsync(It.IsAny<string>()))
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

        private static IAssetsServiceWithCache Given_Customized_AssetServiceWithCacheMock(AlgoClientInstanceData data, bool isNotFound)
        {
            var fixture = new Fixture();
            var result = new Mock<IAssetsServiceWithCache>();

            result.Setup(service => service.TryGetAssetPairAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(isNotFound ? null :
                    fixture.Build<AssetPair>()
                    .With(pair => pair.QuotingAssetId, data.TradedAssetId)
                    .With(pair => pair.Id, data.AssetPairId)
                    .With(pair => pair.Accuracy, AssetAccuracy)
                    .With(pair => pair.IsDisabled, false)
                    .With(pair => pair.MinVolume, MinVolume)
                    .With(pair => pair.MinInvertedVolume, MinVolume)
                    .Create()
                );

            result.Setup(service => service.TryGetAssetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(isNotFound ? null :
                fixture.Build<Asset>()
                    .With(asset => asset.Name, data.TradedAssetId)
                    .With(asset => asset.Id, data.TradedAssetId)
                    .With(asset => asset.Accuracy, AssetAccuracy)
                    .With(asset => asset.IsDisabled, false)
                    .Create()
                );

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

        private static IWalletBalanceService Given_Customized_WalletBalanceServiceMock(bool userHasBothAssetsInWallet)
        {
            var fixture = new Fixture();
            var result = new Mock<IWalletBalanceService>();

            result.Setup(service => service.GetTotalWalletBalanceInBaseAssetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AssetPair>()))
                .ReturnsAsync(100);

            result.Setup(service => service.GetWalletBalancesAsync(It.IsAny<string>(), It.IsAny<AssetPair>()))
                .ReturnsAsync(userHasBothAssetsInWallet ? new List<ClientBalanceResponseModel>
                {
                    fixture.Build<ClientBalanceResponseModel>()
                        .With(w => w.AssetId, TradedAsset)
                        .Create(),

                     fixture.Build<ClientBalanceResponseModel>()
                       .With(w => w.AssetId, QuotingAsset)
                        .Create()

                } : new List<ClientBalanceResponseModel>
                    {
                        fixture.Build<ClientBalanceResponseModel>()
                            .With(w => w.AssetId, TradedAsset)
                            .Create()
                    });


            return result.Object;
        }

        private static List<AlgoClientInstanceData> When_Invoke_GetAllAlgoInstanceDataAsync(AlgoInstancesService service, string clientId, string algoId, out Exception exception)
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
        private static AlgoClientInstanceData When_Invoke_GetAlgoInstanceDataAsync(AlgoInstancesService service, string clientId, string algoId, string id, out Exception exception)
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
        private static AlgoClientInstanceData When_Invoke_SaveAlgoInstanceDataAsync(AlgoInstancesService service, AlgoClientInstanceData data, string algoClientId, out Exception exception)
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
        private static AlgoClientInstanceData When_Invoke_SaveAlgoBackTestInstanceDataAsync(AlgoInstancesService service, AlgoClientInstanceData data, string algoClientId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.SaveAlgoFakeTradingInstanceDataAsync(data, algoClientId).Result;
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

        private static void Then_Data_ShouldNotBe_Empty(List<AlgoData> data)
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

        private static void Then_Data_ShouldBe_Empty(List<AlgoData> data)
        {
            Assert.IsTrue(data.IsNullOrEmptyCollection());
        }

        private static void Then_Data_ShouldNotBe_Empty(AlgoClientInstanceData data)
        {
            Assert.NotNull(data);
        }

        private static void Then_Data_ShouldNotBe_Empty(AlgoDataInformation data)
        {
            Assert.NotNull(data);
        }

        private static void Then_Data_ShouldNotBe_Empty(List<EnumValue> data)
        {
            Assert.NotNull(data);
        }

        private static void Then_Data_ShouldBe_Empty(AlgoDataInformation data)
        {
            Assert.Null(data);
        }

        private void Then_Demo_Instances_Wallet_IsNull(List<UserInstanceData> data)
        {
            foreach (var elem in data.Where(i => i.InstanceType == AlgoInstanceType.Test || i.InstanceType == AlgoInstanceType.Demo))
            {
                Assert.Null(elem.Wallet);
            }
        }

        private void Then_Live_Instances_Have_Wallet(List<UserInstanceData> data)
        {
            foreach (var elem in data.Where(i => i.InstanceType == AlgoInstanceType.Live))
            {
                Assert.NotNull(elem.Wallet);
            }
        }

        private void Then_Data_ShouldNotBe_Null(List<UserInstanceData> data)
        {
            Assert.NotNull(data);
        }

        private static AlgoClientInstanceData Given_AlgoClientInstanceData(double volume, AlgoInstanceType type, bool areDatesCorrect = true)
        {
            var fixture = new Fixture();
            var dtType = typeof(DateTime).FullName;

            var startFromParameter = fixture.Build<AlgoMetaDataParameter>()
                .With(t => t.Type, dtType)
                .With(k => k.Key, "StartFrom")
                .With(v => v.Value, StartFromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))             
                .Create();

            var endOnParameter = fixture.Build<AlgoMetaDataParameter>()
                .With(t => t.Type, dtType)
                .With(k => k.Key, "EndOn")
                .With(v => v.Value, areDatesCorrect ? StartFromDate.AddDays(10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ") :
                    StartFromDate.AddDays(-10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                .Create();

            var startingDateParameter = fixture.Build<AlgoMetaDataParameter>()
                .With(t => t.Type, dtType)
                .With(k => k.Key, "startingDate")
                .With(v => v.Value, StartFromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                .Create();

            var endingDateParameter = fixture.Build<AlgoMetaDataParameter>()
                .With(t => t.Type, dtType)
                .With(k => k.Key, "endingDate")
                .With(v => v.Value, areDatesCorrect ? StartFromDate.AddDays(10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ") :
                    StartFromDate.AddDays(-10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                .Create();

            var metaDataFunction = fixture.Build<AlgoMetaDataFunction>()
                .With(f => f.Parameters, new List<AlgoMetaDataParameter>
                    { startingDateParameter, endingDateParameter })
                .Create();

            var metaDataInformation = fixture.Build<AlgoMetaDataInformation>()
                .With(a => a.Parameters, new List<AlgoMetaDataParameter>
                    { startFromParameter, endOnParameter })
                .With(a => a.Functions, new List<AlgoMetaDataFunction> { metaDataFunction })
                .Create();

            return fixture.Build<AlgoClientInstanceData>()
                .With(a => a.Volume, volume)
                .With(a => a.TradedAssetId, TradedAsset)
                .With(a => a.AssetPairId, AssetPair)
                .With(a => a.AlgoInstanceType, type)
                .With(a => a.AlgoMetaDataInformation, metaDataInformation)
                .Create();
        }

        private static IAssetsServiceWithCache Given_AssetsServiceWithCache()
        {
            var fixture = new Fixture();
            var result = new Mock<IAssetsServiceWithCache>();

            result.Setup(service => service.GetAllAssetPairsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AssetPair>());

            result.Setup(service => service.TryGetAssetPairAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string assetPairId, CancellationToken token) =>
                    fixture.Build<AssetPair>()
                    .With(a => a.Id, assetPairId)
                    .Create());

            result.Setup(service => service.TryGetAssetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string assetId, CancellationToken token) =>
                    fixture.Build<Asset>()
                    .With(a => a.Id, TradedAssetKey)
                    .Create());

            return result.Object;
        }

        #endregion
    }
}
