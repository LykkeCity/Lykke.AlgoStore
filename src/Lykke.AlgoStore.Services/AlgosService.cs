﻿using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.Services.Strings;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.Models;
using Lykke.Service.PersonalData.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Newtonsoft.Json;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;
using BaseAlgoInstance = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.BaseAlgoInstance;
using IAlgoClientInstanceRepository = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories.IAlgoClientInstanceRepository;

namespace Lykke.AlgoStore.Services
{
    public class AlgosService : BaseAlgoStoreService, IAlgosService
    {
        private readonly IAlgoRepository _algoRepository;
        private readonly IAlgoBlobRepository _blobRepository;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly IAlgoRatingsRepository _ratingsRepository;
        private readonly IPublicAlgosRepository _publicAlgosRepository;
        private readonly IStatisticsRepository _statisticsRepository;

        private readonly IAlgoRuntimeDataReadOnlyRepository _runtimeDataRepository; // TODO Should be removed
        private readonly IKubernetesApiReadOnlyClient _kubernetesApiClient;

        private readonly IAssetsService _assetService;
        private readonly IPersonalDataService _personalDataService;
        private readonly IClientAccountClient _clientAccountService;
        private readonly ICandleshistoryservice _candlesHistoryService;
        private readonly AssetsValidator _assetsValidator;
        private readonly IWalletBalanceService _walletBalanceService;
        private readonly ICodeBuildService _codeBuildService;

        private static Random rnd = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgoStoreClientDataService"/> class.
        /// </summary>
        /// <param name="algoRepository">The algo repository.</param>
        /// <param name="runtimeDataRepository">The runtime data repository.</param>
        /// <param name="blobRepository">The BLOB repository.</param>
        /// <param name="instanceRepository">The instance repository.</param>
        /// <param name="ratingsRepository">The ratings repository.</param>
        /// <param name="publicAlgosRepository">The public algos repository.</param>
        /// <param name="statisticsRepository">The statistics repository</param>
        /// <param name="assetService">The asset service.</param>
        /// <param name="personalDataService">The personal Data Service</param>
        /// <param name="kubernetesApiClient">The kubernetes API client.</param>
        /// <param name="clientAccountClient">The Client Account Service</param>
        /// <param name="walletBalanceService">The Wallet Balance Service</param>
        /// <param name="log">The log.</param>
        /// <param name="candlesHistoryService">The Cangles History Service</param>
        /// <param name="assetsValidator">The Asset Validator</param>
        /// <param name="codeBuildService">Algo code validator</param>
        public AlgosService(IAlgoRepository algoRepository,
            IAlgoRuntimeDataReadOnlyRepository runtimeDataRepository,
            IAlgoBlobRepository blobRepository,
            IAlgoClientInstanceRepository instanceRepository,
            IAlgoRatingsRepository ratingsRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IStatisticsRepository statisticsRepository,
            IAssetsService assetService,
            IPersonalDataService personalDataService,
            IKubernetesApiReadOnlyClient kubernetesApiClient,
            IClientAccountClient clientAccountClient,
            ICandleshistoryservice candlesHistoryService,
            [NotNull] AssetsValidator assetsValidator,
            IWalletBalanceService walletBalanceService,
            ILog log,
            ICodeBuildService codeBuildService) : base(log, nameof(AlgosService))
        {
            _algoRepository = algoRepository;
            _runtimeDataRepository = runtimeDataRepository;
            _blobRepository = blobRepository;
            _instanceRepository = instanceRepository;
            _ratingsRepository = ratingsRepository;
            _publicAlgosRepository = publicAlgosRepository;
            _statisticsRepository = statisticsRepository;
            _assetService = assetService;
            _personalDataService = personalDataService;
            _kubernetesApiClient = kubernetesApiClient;
            _clientAccountService = clientAccountClient;
            _candlesHistoryService = candlesHistoryService;
            _assetsValidator = assetsValidator;
            _walletBalanceService = walletBalanceService;
            _codeBuildService = codeBuildService;
        }

        /// <summary>
        /// Gets all algos with rating asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<List<AlgoRatingMetaData>> GetAllAlgosWithRatingAsync()
        {
            return await LogTimedInfoAsync(nameof(GetAllAlgosWithRatingAsync), null, async () =>
            {
                var result = new List<AlgoRatingMetaData>();

                var algos = await _publicAlgosRepository.GetAllPublicAlgosAsync();

                if (algos.IsNullOrEmptyCollection())
                    return result;

                foreach (var publicAlgo in algos)
                {
                    var currentAlgo = await _algoRepository.GetAlgoAsync(publicAlgo.ClientId, publicAlgo.AlgoId);

                    if (currentAlgo == null)
                        continue;

                    string authorName;

                    if (String.IsNullOrEmpty(currentAlgo.ClientId))
                        authorName = "Administrator";
                    else
                    {
                        var authorPersonalData = await _personalDataService.GetAsync(currentAlgo.ClientId);
                        authorName = !String.IsNullOrEmpty(authorPersonalData.FullName)
                                                        ? authorPersonalData.FullName
                                                        : authorPersonalData.Email;
                    }

                    var ratingMetaData = new AlgoRatingMetaData
                    {
                        ClientId = currentAlgo.ClientId,
                        AlgoId = currentAlgo.AlgoId,
                        Name = currentAlgo.Name,
                        Description = currentAlgo.Description,
                        Date = currentAlgo.Date,
                        Author = authorName
                    };

                    var rating = await _ratingsRepository.GetAlgoRatingsAsync(currentAlgo.AlgoId);
                    if (rating != null && rating.Count > 0)
                    {
                        ratingMetaData.Rating = Math.Round(rating.Average(item => item.Rating), 2);
                        ratingMetaData.RatedUsersCount = rating.Count;
                    }
                    else
                    {
                        ratingMetaData.Rating = 0;
                        ratingMetaData.RatedUsersCount = 0;
                    }

                    ratingMetaData.UsersCount = rnd.Next(1, 500); // TODO hardcoded until real count is displayed                        
                    result.Add(ratingMetaData);
                }

                return result;
            });
        }

        /// <summary>
        /// Save the rating for a specific algo for a specific user
        /// </summary>
        /// <param name="data">Ratings data</param>
        /// <returns></returns>
        public async Task<AlgoRatingData> SaveAlgoRatingAsync(AlgoRatingData data)
        {
            return await LogTimedInfoAsync(nameof(SaveAlgoRatingAsync), data.ClientId, async () =>
            {
                if (string.IsNullOrEmpty(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientID is empty.");

                if (string.IsNullOrEmpty(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId is empty.");

                if (double.IsNaN(data.Rating))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "Invalid rating.");

                await _ratingsRepository.SaveAlgoRatingAsync(data);

                var newRatingInformation = await _ratingsRepository.GetAlgoRatingsAsync(data.AlgoId);
                var newRatingData = new AlgoRatingData
                {
                    AlgoId = data.AlgoId,
                    ClientId = data.ClientId,
                    RatedUsersCount = newRatingInformation.Count,
                    Rating = Math.Round(newRatingInformation.Average(item => item.Rating), 2)
                };

                return newRatingData;
            });
        }

        /// <summary>
        /// Get the rating of a specific client for an algo
        /// </summary>
        /// <param name="algoId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoRatingForClientAsync), clientId, async () =>
            {
                if (string.IsNullOrEmpty(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientID is empty.");

                if (string.IsNullOrEmpty(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId is empty.");

                var result = await _ratingsRepository.GetAlgoRatingForClientAsync(algoId, clientId);

                return result;
            });
        }

        /// <summary>
        /// Get the average rating for an algo
        /// </summary>
        /// <param name="algoId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<AlgoRatingData> GetAlgoRatingAsync(string algoId, string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoRatingAsync), clientId, async () =>
            {
                if (string.IsNullOrEmpty(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId is empty.");

                var ratings = await _ratingsRepository.GetAlgoRatingsAsync(algoId);

                var result = new AlgoRatingData
                {
                    AlgoId = algoId,
                    ClientId = clientId,
                };


                if (ratings != null && ratings.Count > 0)
                {
                    result.Rating = Math.Round(ratings.Average(item => item.Rating), 2);
                    result.RatedUsersCount = ratings.Count;
                }
                else
                {
                    result.Rating = 0;
                    result.RatedUsersCount = 0;
                }

                return result;
            });
        }

        /// <summary>
        /// Create an algo from provided code
        /// </summary>
        /// <param name="clientId">Algo client Id</param>
        /// <param name="clientName">Algo client name</param>
        /// <param name="data">Algo data</param>
        /// <param name="algoContent">Algo code content</param>
        /// <returns></returns>
        public async Task<AlgoData> CreateAlgoAsync(AlgoData data, string algoContent)
        {
            return await LogTimedInfoAsync(nameof(CreateAlgoAsync), data.ClientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.ClientIdEmpty);

                if(string.IsNullOrEmpty(algoContent))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.AlgoContentEmpty);

                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    data.AlgoId = Guid.NewGuid().ToString();

                if (!data.ValidateData(out var exception))
                    throw exception;

                //Validate algo code
                var validationSession = _codeBuildService.StartSession(algoContent);
                var validationResult = await validationSession.Validate();

                if (!validationResult.IsSuccessful)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        string.Format(Phrases.AlgoDataSaveFailedOnCodeValidation, Environment.NewLine, data.ClientId, data.AlgoId, validationResult));

                //Extract algo metadata (parameters)
                var extractedMetadata = await validationSession.ExtractMetadata();

                data.AlgoMetaDataInformationJSON = JsonConvert.SerializeObject(extractedMetadata);

                var algoToSave = AutoMapper.Mapper.Map<IAlgo>(data);

                await _algoRepository.SaveAlgoAsync(algoToSave);

                var res = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                if (res == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        string.Format(Phrases.AlgoDataSaveFailed, data.ClientId, data.AlgoId));

                await _blobRepository.SaveBlobAsync(data.AlgoId, algoContent);

                return AutoMapper.Mapper.Map<AlgoData>(res);
            });
        }

        /// <summary>
        /// Edit (update) an algo from provided code
        /// </summary>
        /// <param name="clientId">Algo client Id</param>
        /// <param name="clientName">Algo client name</param>
        /// <param name="data">Algo data</param>
        /// <param name="algoContent">Algo code content</param>
        /// <returns>Algo client meta data that is updated</returns>
        public async Task<AlgoData> EditAlgoAsync(AlgoData data, string algoContent)
        {
            return await LogTimedInfoAsync(nameof(CreateAlgoAsync), data.ClientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.ClientIdEmpty);

                if (string.IsNullOrEmpty(algoContent))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.AlgoContentEmpty);

                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.AlgoIdEmpty);

                if (!data.ValidateData(out var exception))
                    throw exception;

                var res = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                //Algo should not be public in order to edit it
                if (res == null || res.AlgoVisibility == AlgoVisibility.Public)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        string.Format(Phrases.NoAlgoData, data.ClientId, data.AlgoId));

                //Check if there are running algo instances
                var instances = await _instanceRepository.GetAllAlgoInstancesByAlgoAsync(data.AlgoId);

                if (instances.Any(x => x.AlgoInstanceStatus == AlgoInstanceStatus.Started))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.RunningAlgoInstanceExists);

                //Validate algo code
                var validationSession = _codeBuildService.StartSession(algoContent);
                var validationResult = await validationSession.Validate();

                if (!validationResult.IsSuccessful)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        string.Format(Phrases.AlgoDataSaveFailedOnCodeValidation, Environment.NewLine, data.ClientId, data.AlgoId, validationResult));

                //Extract algo metadata (parameters)
                var extractedMetadata = await validationSession.ExtractMetadata();

                data.AlgoMetaDataInformationJSON = JsonConvert.SerializeObject(extractedMetadata);

                var algoToSave = AutoMapper.Mapper.Map<IAlgo>(data);

                await _algoRepository.SaveAlgoAsync(algoToSave);
                await _blobRepository.SaveBlobAsync(data.AlgoId, algoContent);

                res = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                return AutoMapper.Mapper.Map<AlgoData>(res);
            });
        }

        /// <summary>
        /// Gets the client metadata asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public async Task<AlgoDataInformation> GetAlgoDataInformationAsync(string clientId, string algoId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoDataInformationAsync), clientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");
                if (string.IsNullOrWhiteSpace(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId Is empty");

                var algoInformation = await _algoRepository.GetAlgoDataInformationAsync(clientId, algoId);

                var rating = await _ratingsRepository.GetAlgoRatingsAsync(algoId);

                if (algoInformation != null)
                {

                    if (rating != null && rating.Count > 0)
                    {
                        algoInformation.Rating = Math.Round(rating.Average(item => item.Rating), 2);
                        algoInformation.RatedUsersCount = rating.Count;
                    }
                    else
                    {
                        algoInformation.Rating = 0;
                        algoInformation.RatedUsersCount = 0;
                    }

                    algoInformation.UsersCount = rnd.Next(1, 500); // TODO hardcoded until real count is displayed                        

                    algoInformation.Author = (await _personalDataService.GetAsync(clientId))?.FullName;
                }
                return algoInformation;
            });
        }

        /// <summary>
        /// Adds algo to public algos asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="clientId">The id of the logget user</param>
        /// <returns></returns>
        public async Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data, string clientId)
        {
            return await LogTimedInfoAsync(nameof(AddToPublicAsync), clientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");
                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId Is empty");
                if (data.ClientId != clientId)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.Unauthorized,
                        $"User with id {clientId} cannot publish algo because he/she is not the author.",
                        Phrases.UserNotAuthorOfAlgo);

                await _publicAlgosRepository.SavePublicAlgoAsync(data);

                var algo = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                algo.AlgoVisibility = Core.Enumerators.AlgoVisibility.Public;

                await _algoRepository.SaveAlgoAsync(algo);

                return data;
            });
        }

        /// <summary>
        /// Remove algo from public algos asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="clientId">The id of the logget user</param>
        /// <returns></returns>
        public async Task<PublicAlgoData> RemoveFromPublicAsync(PublicAlgoData data, string clientId)
        {
            return await LogTimedInfoAsync(nameof(RemoveFromPublicAsync), clientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");
                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId Is empty");
                if (data.ClientId != clientId)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.Unauthorized,
                        $"User with id {clientId} cannot unpublish algo because he/she is not the author.",
                        Phrases.UserNotAuthorOfAlgo);

                bool algoExists = await _publicAlgosRepository.ExistsPublicAlgoAsync(data.ClientId, data.AlgoId);

                if (!algoExists)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotPublic,
                        $"There is no public algo with AlgoId {data.AlgoId}, Client id {data.ClientId}",
                        Phrases.PublicAlgoNotFound);

                var algoInstances = await _instanceRepository.GetAllAlgoInstancesByAlgoAsync(data.AlgoId);

                if (algoInstances.Any())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.UnableToDeleteData,
                        $"Cannot unpublish algo because it has algo instances. Algo id {data.AlgoId}, Client id {data.ClientId}",
                        Phrases.AlgoInstancesExist);

                await _publicAlgosRepository.DeletePublicAlgoAsync(data);

                var algo = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                algo.AlgoVisibility = Core.Enumerators.AlgoVisibility.Private;

                await _algoRepository.SaveAlgoAsync(algo);

                return data;
            });
        }

        /// <summary>
        /// Deletes the metadata asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task DeleteAsync(ManageImageData data)
        {
            await LogTimedInfoAsync(nameof(DeleteAsync), data?.ClientId, async () =>
            {
                if (await _publicAlgosRepository.ExistsPublicAlgoAsync(data.ClientId, data.AlgoId) ||
                   await _instanceRepository.HasInstanceData(data.ClientId, data.AlgoId))
                    return;

                if (await _blobRepository.BlobExistsAsync(data.AlgoId))
                    await _blobRepository.DeleteBlobAsync(data.AlgoId);

                await _algoRepository.DeleteAlgoAsync(data.ClientId, data.AlgoId);
            });
        }

        /// <summary>
        /// Saves the algo as binary asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="dataModel">The data model.</param>
        /// <returns></returns>
        public async Task SaveAlgoAsBinaryAsync(string clientId, UploadAlgoBinaryData dataModel)
        {
            await LogTimedInfoAsync(nameof(SaveAlgoAsBinaryAsync), clientId, async () =>
            {
                if (!dataModel.ValidateData(out var exception))
                    throw exception;

                var algo = await _algoRepository.GetAlgoAsync(clientId, dataModel.AlgoId);
                if (algo == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Specified algo id {dataModel.AlgoId} is not found! Cant save file for a non existing algo.");

                await _blobRepository.SaveBlobAsync(dataModel.AlgoId, dataModel.Data.OpenReadStream());
            });
        }
        /// <summary>
        /// Saves the algo as string asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="dataModel">The data model.</param>
        /// <returns></returns>
        public async Task SaveAlgoAsStringAsync(string clientId, UploadAlgoStringData dataModel)
        {
            await LogTimedInfoAsync(nameof(SaveAlgoAsStringAsync), clientId, async () =>
            {
                if (!dataModel.ValidateData(out var exception))
                    throw exception;

                var algo = await _algoRepository.GetAlgoAsync(clientId, dataModel.AlgoId);
                if (algo == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Specified algo id {dataModel.AlgoId} is not found! Can't save string for a non existing algo.");

                await _blobRepository.SaveBlobAsync(dataModel.AlgoId, dataModel.Data);
            });
        }
        /// <summary>
        /// Gets the algo as string asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="algoId">The algo identifier.</param>
        /// <returns></returns>
        public async Task<string> GetAlgoAsStringAsync(string clientId, string algoId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoAsStringAsync), clientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");
                if (string.IsNullOrWhiteSpace(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId Is empty");

                var algo = await _algoRepository.GetAlgoAsync(clientId, algoId);
                if (algo == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Specified algo id {algoId} is not found!");

                return await _blobRepository.GetBlobStringAsync(algoId);
            });
        }


        public async Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataByClientIdAsync(string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetAllAlgoInstanceDataByClientIdAsync), clientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");

                var result = await _instanceRepository.GetAllAlgoInstancesByClientAsync(clientId);

                return result.ToList();
            });
        }

        /// <summary>
        /// Saves the algo instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="algoClientId">Algo client id.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> SaveAlgoInstanceDataAsync(AlgoClientInstanceData data, string algoClientId)
        {
            return await LogTimedInfoAsync(nameof(SaveAlgoInstanceDataAsync), data.ClientId,
                async () => await SaveInstanceDataAsync(data, algoClientId));
        }

        /// <summary>
        /// Saves the algo back-test instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="algoClientId">Algo client id.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> SaveAlgoBackTestInstanceDataAsync(AlgoClientInstanceData data, string algoClientId)
        {
            return await LogTimedInfoAsync(nameof(SaveAlgoBackTestInstanceDataAsync), data.ClientId,
                async () => await SaveInstanceDataAsync(data, algoClientId, true));
        }

        /// <summary>
        /// Create statistics summary and save initial wallet status in it
        /// </summary>
        /// <param name="data">The algo instance data</param>
        /// <param name="assetPair">The asset pair for the algo instance</param>
        /// <param name="tradedAsset">The traded asset from the Asset pair</param>
        /// <param name="assetTwo">The second asset from the Asset pair</param>
        private async Task SaveSummaryStatistic(AlgoClientInstanceData data, AssetPair assetPair, Asset tradedAsset, Asset assetTwo)
        {
            double clientTradedAssetBalance;
            double clientAssetTwoBalance;
            double initialWalletBalance;
            string userCurrencyAssetId = null;

            if (data.AlgoInstanceType == CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceType.Test)
            {
                userCurrencyAssetId = assetPair.QuotingAssetId;

                clientTradedAssetBalance = data.BackTestTradingAssetBalance;
                clientAssetTwoBalance = data.BackTestAssetTwoBalance;

                var tradedAssetBalanceAbsoluteValue = await _candlesHistoryService.GetCandlesHistoryAsync(assetPair.Id,
                    Lykke.Service.CandlesHistory.Client.Models.CandlePriceType.Mid,
                    Lykke.Service.CandlesHistory.Client.Models.CandleTimeInterval.Day,
                    DateTime.Parse(data.AlgoMetaDataInformation.Parameters.First(p => p.Key == "StartFrom").Value),
                    DateTime.Parse(data.AlgoMetaDataInformation.Parameters.First(p => p.Key == "StartFrom").Value));

                if (!tradedAssetBalanceAbsoluteValue.History.Any())
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InitialWalletBalanceNotCalculated,
                        $"Initial wallet balance could not be calculated. Could not get history price for {assetPair.Name}");
                }
                //show balance for the quoting asset from the Asset pair - for back test
                if (data.IsStraight)
                    initialWalletBalance = clientAssetTwoBalance + tradedAssetBalanceAbsoluteValue.History.First().Close * clientTradedAssetBalance;
                else
                    initialWalletBalance = clientTradedAssetBalance + tradedAssetBalanceAbsoluteValue.History.First().Close * clientAssetTwoBalance;

            }
            else
            {
                var baseUserAssetId = await GetBaseAssetAsync(data.ClientId);
                var assetResponse = await _assetService.AssetGetWithHttpMessagesAsync(baseUserAssetId.BaseAssetId);

                if (assetResponse == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"There is no asset with an id {baseUserAssetId.BaseAssetId}");

                userCurrencyAssetId = baseUserAssetId.BaseAssetId;

                var walletBalances = await _walletBalanceService.GetWalletBalancesAsync(data.WalletId, assetPair);
                initialWalletBalance = await _walletBalanceService.GetTotalWalletBalanceInBaseAssetAsync(data.WalletId, baseUserAssetId.BaseAssetId, assetPair);

                var clientBalanceResponseModels = walletBalances.ToList();
                clientTradedAssetBalance = clientBalanceResponseModels.First(b => b.AssetId == tradedAsset.Id).Balance;
                clientAssetTwoBalance = clientBalanceResponseModels.First(b => b.AssetId != tradedAsset.Id).Balance;
            }

            await _statisticsRepository.CreateOrUpdateSummaryAsync(new StatisticsSummary
            {
                InitialWalletBalance = initialWalletBalance,
                InitialTradedAssetBalance = clientTradedAssetBalance,
                InitialAssetTwoBalance = clientAssetTwoBalance,
                LastTradedAssetBalance = clientTradedAssetBalance,
                LastAssetTwoBalance = clientAssetTwoBalance,
                TradedAssetName = tradedAsset.Name,
                AssetTwoName = assetTwo.Name,
                InstanceId = data.InstanceId,
                LastWalletBalance = initialWalletBalance,
                TotalNumberOfStarts = 0,
                TotalNumberOfTrades = 0,
                UserCurrencyBaseAssetId = userCurrencyAssetId
            });

            var statisticsSummaryResult = await _statisticsRepository.GetSummaryAsync(data.InstanceId);
            if (statisticsSummaryResult == null)
                throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                    $"Could not save summary row for AlgoInstance: {data.InstanceId}, User: {data.ClientId} AlgoId: {data.AlgoId}");
        }

        private async Task<WalletDtoModel> GetClientWallet(string clientId, string walletId)
        {
            var wallets = await _clientAccountService.GetWalletsByClientIdAsync(clientId);
            return wallets?.FirstOrDefault(x => x.Id == walletId);
        }

        /// <summary>
        /// Get the base asset Id for the user
        /// </summary>
        /// <param name="clientId">User Id</param>
        /// <returns></returns>
        private async Task<BaseAssetClientModel> GetBaseAssetAsync(string clientId)
        {
            var baseAsset = await _clientAccountService.GetBaseAssetAsync(clientId);
            if (baseAsset == null)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.AssetNotFound,
                    $"Base asset for user {clientId} not found");
            }

            return baseAsset;
        }

        /// <summary>
        /// Check if the wallet is used by another running algo instance of the user.
        /// </summary>
        /// <param name="walletId">
        /// Wallet id that the user wants to use for trading
        /// </param>
        private async Task<bool> IsWalletUsedByExistingStartedInstance(string walletId)
        {
            var algoInstances = (await _instanceRepository.GetAllByWalletIdAndInstanceStatusIsNotStoppedAsync(walletId));
            return algoInstances != null && algoInstances.Count() > 0;
        }

        private async Task<AlgoClientInstanceData> SaveInstanceDataAsync(
            AlgoClientInstanceData data,
            string algoClientId,
            bool isBackTestInstance = false)
        {
            if (string.IsNullOrWhiteSpace(data.InstanceId))
                data.InstanceId = Guid.NewGuid().ToString();

            if (!data.ValidateData(out var exception))
                throw exception;

            if (!isBackTestInstance)
            {
                var wallet = await GetClientWallet(data.ClientId, data.WalletId);
                if (wallet == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.WalletNotFound,
                        $"Wallet {data.WalletId} not found for client {data.ClientId}");

                if (!string.IsNullOrEmpty(data.WalletId) && await IsWalletUsedByExistingStartedInstance(data.WalletId))
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.WalletIsAlreadyUsed,
                        string.Format(Phrases.WalletIsAlreadyUsed, data.WalletId, data.AlgoId, data.ClientId),
                        Phrases.WalletAlreadyUsed);
                }
            }

            if (!await _algoRepository.ExistsAlgoAsync(algoClientId, data.AlgoId))
                throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                    $"Algo {data.AlgoId} no found for client {data.ClientId}");

            if (algoClientId != data.ClientId && !await _publicAlgosRepository.ExistsPublicAlgoAsync(algoClientId, data.AlgoId))
                throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotPublic,
                    $"Algo {data.AlgoId} not public for client {data.ClientId}");

            var assetPairResponse = await _assetService.AssetPairGetWithHttpMessagesAsync(data.AssetPair);
            _assetsValidator.ValidateAssetPairResponse(assetPairResponse);
            _assetsValidator.ValidateAssetPair(data.AssetPair, assetPairResponse.Body);

            var baseAsset = await _assetService.AssetGetWithHttpMessagesAsync(assetPairResponse.Body.BaseAssetId);
            _assetsValidator.ValidateAssetResponse(baseAsset);

            var quotingAsset = await _assetService.AssetGetWithHttpMessagesAsync(assetPairResponse.Body.QuotingAssetId);
            _assetsValidator.ValidateAssetResponse(quotingAsset);
            _assetsValidator.ValidateAsset(assetPairResponse.Body, data.TradedAsset, baseAsset.Body, quotingAsset.Body);

            var straight = data.TradedAsset == baseAsset.Body.Id || data.TradedAsset == baseAsset.Body.Name;

            //get traded asset
            var asset = straight ? baseAsset : quotingAsset;

            _assetsValidator.ValidateAccuracy(data.Volume, asset.Body.Accuracy);

            var volume = data.Volume.TruncateDecimalPlaces(asset.Body.Accuracy);
            var minVolume = straight ? assetPairResponse.Body.MinVolume : assetPairResponse.Body.MinInvertedVolume;
            _assetsValidator.ValidateVolume(volume, minVolume, asset.Body.DisplayId);

            if (!isBackTestInstance)
                _walletBalanceService.ValidateWallet(data.WalletId, assetPairResponse.Body);

            if (string.IsNullOrEmpty(data.AuthToken))
                data.AuthToken = Guid.NewGuid().ToString();

            data.IsStraight = straight;
            data.OppositeAssetId = straight ? quotingAsset.Body.Id : baseAsset.Body.Id;
            await _instanceRepository.SaveAlgoInstanceDataAsync(data);

            var res = await _instanceRepository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId);
            if (res == null)
                throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                    $"Cannot save {(isBackTestInstance ? "back test" : "")} algo instance data for {data.ClientId} id: {data.AlgoId}");

            await SaveSummaryStatistic(data, assetPairResponse.Body, asset.Body, straight ? quotingAsset.Body : baseAsset.Body);

            return res;
        }
    }
}