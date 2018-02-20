﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Service.Assets.Client;
using Lykke.Service.PersonalData.Contract;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreClientDataService : BaseAlgoStoreService, IAlgoStoreClientDataService
    {
        private readonly IAlgoMetaDataRepository _metaDataRepository;
        private readonly IAlgoBlobRepository _blobRepository;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly IAlgoRatingsRepository _ratingsRepository;
        private readonly IPublicAlgosRepository _publicAlgosRepository;

        private readonly IAlgoRuntimeDataReadOnlyRepository _runtimeDataRepository; // TODO Should be removed
        private readonly IKubernetesApiReadOnlyClient _kubernetesApiClient;

        private readonly IAssetsService _assetService;
        private readonly IPersonalDataService _personalDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgoStoreClientDataService"/> class.
        /// </summary>
        /// <param name="metaDataRepository">The meta data repository.</param>
        /// <param name="runtimeDataRepository">The runtime data repository.</param>
        /// <param name="blobRepository">The BLOB repository.</param>
        /// <param name="instanceRepository">The instance repository.</param>
        /// <param name="ratingsRepository">The ratings repository.</param>
        /// <param name="publicAlgosRepository">The public algos repository.</param>
        /// <param name="assetService">The asset service.</param>
        /// <param name="kubernetesApiClient">The kubernetes API client.</param>
        /// <param name="log">The log.</param>
        public AlgoStoreClientDataService(IAlgoMetaDataRepository metaDataRepository,
            IAlgoRuntimeDataReadOnlyRepository runtimeDataRepository,
            IAlgoBlobRepository blobRepository,
            IAlgoClientInstanceRepository instanceRepository,
            IAlgoRatingsRepository ratingsRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IAssetsService assetService,
            IPersonalDataService personalDataService,
            IKubernetesApiReadOnlyClient kubernetesApiClient,
            ILog log) : base(log, nameof(AlgoStoreClientDataService))
        {
            _metaDataRepository = metaDataRepository;
            _runtimeDataRepository = runtimeDataRepository;
            _blobRepository = blobRepository;
            _instanceRepository = instanceRepository;
            _ratingsRepository = ratingsRepository;
            _publicAlgosRepository = publicAlgosRepository;
            _assetService = assetService;
            _personalDataService = personalDataService;
            _kubernetesApiClient = kubernetesApiClient;
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
                    var currentAlgoMetadata = await _metaDataRepository.GetAlgoMetaDataAsync(publicAlgo.ClientId, publicAlgo.AlgoId);
                    if (currentAlgoMetadata.Author == null) currentAlgoMetadata.Author = "Administrator";

                    foreach (var algoMetadata in currentAlgoMetadata.AlgoMetaData)
                    {
                        var ratingMetaData = new AlgoRatingMetaData
                        {
                            ClientId = currentAlgoMetadata.ClientId,
                            AlgoId = algoMetadata.AlgoId,
                            Name = algoMetadata.Name,
                            Description = algoMetadata.Description,
                            Date = algoMetadata.Date,
                            Author = currentAlgoMetadata.Author
                        };

                        var rating = _ratingsRepository.GetAlgoRating(currentAlgoMetadata.ClientId, algoMetadata.AlgoId);
                        if (rating != null)
                        {
                            ratingMetaData.Rating = rating.Rating;
                            ratingMetaData.UsersCount = rating.UsersCount;
                        }

                        result.Add(ratingMetaData);
                    }

                }

                return result;
            });
        }

        /// <summary>
        /// Gets the client metadata asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public async Task<AlgoClientMetaData> GetClientMetadataAsync(string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetClientMetadataAsync), clientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");

                var algos = await _metaDataRepository.GetAllClientAlgoMetaDataAsync(clientId);

                if (algos == null || algos.AlgoMetaData.IsNullOrEmptyCollection())
                    return algos;

                foreach (var metadata in algos.AlgoMetaData)
                {
                    var runtimeData = await _runtimeDataRepository.GetAlgoRuntimeDataAsync(clientId, metadata.AlgoId);

                    if (runtimeData == null)
                    {
                        metadata.Status = AlgoRuntimeStatuses.Unknown.ToUpperText();
                        // TODO Skip?!?
                        continue;
                    }

                    var status = ClientAlgoRuntimeStatuses.NotFound.ToUpperText();
                    try
                    {
                        var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(metadata.AlgoId);
                        if (!pods.IsNullOrEmptyCollection())
                        {
                            if (pods.Count != 1)
                                throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound,
                                    $"More than one pod for algoId {metadata.AlgoId}");

                            var pod = pods[0];
                            if (pod != null)
                            {
                                status = pod.Status.Phase.ToUpper();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, ex).Wait();
                    }
                    metadata.Status = status;
                }
                return algos;
            });
        }

        public async Task<AlgoClientMetaDataInformation> GetAlgoMetaDataInformationAsync(string clientId, string algoId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoMetaDataInformationAsync), clientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");
                if (string.IsNullOrWhiteSpace(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AlgoId Is empty");

                var algoInformation = await _metaDataRepository.GetAlgoMetaDataInformationAsync(clientId, algoId);

                var rating = _ratingsRepository.GetAlgoRating(clientId, algoId);

                if (algoInformation != null)
                {
                    if (rating != null)
                    {
                        algoInformation.Rating = rating.Rating;
                        algoInformation.UsersCount = rating.UsersCount;
                    }

                    algoInformation.Author = (await _personalDataService.GetAsync(clientId))?.FullName;
                }
                return algoInformation;
            });
        }

        /// <summary>
        /// Adds algo to public algos asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data)
        {
            return await LogTimedInfoAsync(nameof(AddToPublicAsync), data.ClientId, async () =>
            {
                await _publicAlgosRepository.SavePublicAlgoAsync(data);

                return data;
            });
        }

        /// <summary>
        /// Validates the cascade delete client metadata request asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> ValidateCascadeDeleteClientMetadataRequestAsync(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(ValidateCascadeDeleteClientMetadataRequestAsync), data?.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _metaDataRepository.ExistsAlgoMetaDataAsync(data.ClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Algo metadata not found for {data.AlgoId}");

                var result = await _instanceRepository.GetAlgoInstanceDataAsync(data.ClientId, data.AlgoId, data.InstanceId);
                if (result == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound,
                        $"Algo instance data not found for {data.InstanceId}");

                if (!result.ValidateData(out var instanceException))
                    throw instanceException;

                return result;
            });
        }
        /// <summary>
        /// Saves the client metadata asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientName">Name of the client.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<AlgoClientMetaData> SaveClientMetadataAsync(string clientId, string clientName, AlgoMetaData data)
        {
            return await LogTimedInfoAsync(nameof(SaveClientMetadataAsync), clientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "ClientId Is empty");

                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    data.AlgoId = Guid.NewGuid().ToString();

                if (!data.ValidateData(out var exception))
                    throw exception;

                var clientData = new AlgoClientMetaData
                {
                    ClientId = clientId,
                    Author = clientName,
                    AlgoMetaData = new List<AlgoMetaData>
                    {
                        data
                    }
                };
                await _metaDataRepository.SaveAlgoMetaDataAsync(clientData);

                var res = await _metaDataRepository.GetAlgoMetaDataAsync(clientId, data.AlgoId);
                if (res == null || res.AlgoMetaData.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        $"Cannot save data for {clientId} id: {data.AlgoId}");

                return res;
            });
        }
        /// <summary>
        /// Deletes the metadata asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task DeleteMetadataAsync(ManageImageData data)
        {
            await LogTimedInfoAsync(nameof(DeleteMetadataAsync), data?.ClientId, async () =>
            {
                if (await _publicAlgosRepository.ExistsPublicAlgoAsync(data.ClientId, data.AlgoId) ||
                    await _instanceRepository.HasInstanceData(data.ClientId, data.AlgoId))
                    return;

                if (await _blobRepository.BlobExistsAsync(data.AlgoId))
                    await _blobRepository.DeleteBlobAsync(data.AlgoId);

                await _metaDataRepository.DeleteAlgoMetaDataAsync(data.ClientId, data.AlgoId);
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

                var algo = await _metaDataRepository.GetAlgoMetaDataAsync(clientId, dataModel.AlgoId);
                if (algo == null || algo.AlgoMetaData.IsNullOrEmptyCollection())
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

                var algo = await _metaDataRepository.GetAlgoMetaDataAsync(clientId, dataModel.AlgoId);
                if (algo == null || algo.AlgoMetaData.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Specified algo id {dataModel.AlgoId} is not found! Cant save string for a non existing algo.");

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

                var algo = await _metaDataRepository.GetAlgoMetaDataAsync(clientId, algoId);
                if (algo == null || algo.AlgoMetaData.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Specified algo id {algoId} is not found!");

                return await _blobRepository.GetBlobStringAsync(algoId);
            });
        }

        /// <summary>
        /// Gets all algo instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<List<AlgoClientInstanceData>> GetAllAlgoInstanceDataAsync(BaseAlgoData data)
        {
            return await LogTimedInfoAsync(nameof(GetAllAlgoInstanceDataAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                return await _instanceRepository.GetAllAlgoInstanceDataAsync(data.ClientId, data.AlgoId);
            });
        }
        /// <summary>
        /// Gets the algo instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> GetAlgoInstanceDataAsync(BaseAlgoInstance data)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoInstanceDataAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                return await _instanceRepository.GetAlgoInstanceDataAsync(data.ClientId, data.AlgoId, data.InstanceId);
            });
        }
        /// <summary>
        /// Saves the algo instance data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<AlgoClientInstanceData> SaveAlgoInstanceDataAsync(AlgoClientInstanceData data)
        {
            return await LogTimedInfoAsync(nameof(SaveAlgoInstanceDataAsync), data.ClientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(data.InstanceId))
                    data.InstanceId = Guid.NewGuid().ToString();

                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _metaDataRepository.ExistsAlgoMetaDataAsync(data.ClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"Algo {data.AlgoId} no found for client {data.ClientId}");

                var assetPairResponse = await _assetService.AssetPairGetWithHttpMessagesAsync(data.AssetPair);
                if (assetPairResponse.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AssetNotFound, $"AssetPair: {data.AssetPair} was not found");
                if (assetPairResponse.Response.StatusCode != HttpStatusCode.OK)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Invalid response code: {assetPairResponse.Response.StatusCode} from asset service calling AssetPairGetWithHttpMessagesAsync");

                var assetPair = assetPairResponse.Body;
                if (assetPair == null || assetPair.IsDisabled)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "AssetPair is not valid");

                if (assetPair.QuotingAssetId != data.TradedAsset)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"Traded Asset {data.TradedAsset} is not valid - should be {assetPair.QuotingAssetId}");

                if (data.Volume.GetAccuracy() > assetPair.Accuracy)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "Volume accuracy is not valid for this Asset");

                await _instanceRepository.SaveAlgoInstanceDataAsync(data);

                var res = await _instanceRepository.GetAlgoInstanceDataAsync(data.ClientId, data.AlgoId, data.InstanceId);
                if (res == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        $"Cannot save data for {data.ClientId} id: {data.AlgoId}");

                return res;
            });
        }
    }
}
