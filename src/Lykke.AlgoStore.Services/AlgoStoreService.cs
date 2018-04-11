﻿using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.AlgoStore.TeamCityClient.Models;
using Newtonsoft.Json;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;
using IAlgoClientInstanceRepository = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories.IAlgoClientInstanceRepository;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreService : BaseAlgoStoreService, IAlgoStoreService
    {
        private readonly IUserLogRepository _userLogRepository;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IAlgoMetaDataReadOnlyRepository _algoMetaDataRepository;
        private readonly IAlgoBlobReadOnlyRepository _algoBlobRepository;
        private readonly IAlgoClientInstanceRepository _algoInstanceRepository;

        private readonly IStorageConnectionManager _storageConnectionManager;
        private readonly ITeamCityClient _teamCityClient;

        private readonly IKubernetesApiClient _kubernetesApiClient;

        private readonly IPublicAlgosRepository _publicAlgosRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgoStoreService"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="algoBlobRepository">The algo BLOB repository.</param>
        /// <param name="algoMetaDataRepository">The algo meta data repository.</param>
        /// <param name="storageConnectionManager">The storage connection manager.</param>
        /// <param name="teamCityClient">The team city client.</param>
        /// <param name="kubernetesApiClient">The Kubernetes API client.</param>
        /// <param name="algoInstanceRepository">The algo instance repository.</param>
        /// <param name="publicAlgosRepository">The public algo repository.</param>
        /// <param name="statisticsRepository">The statistics repository.</param>
        /// <param name="userLogRepository">The user log repository.</param>
        public AlgoStoreService(
            ILog log,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IStorageConnectionManager storageConnectionManager,
            ITeamCityClient teamCityClient,
            IKubernetesApiClient kubernetesApiClient,
            IAlgoClientInstanceRepository algoInstanceRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IStatisticsRepository statisticsRepository,
            IUserLogRepository userLogRepository) : base(log, nameof(AlgoStoreService))
        {
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
            _storageConnectionManager = storageConnectionManager;
            _teamCityClient = teamCityClient;
            _kubernetesApiClient = kubernetesApiClient;
            _algoInstanceRepository = algoInstanceRepository;
            _publicAlgosRepository = publicAlgosRepository;
            _statisticsRepository = statisticsRepository;
            _userLogRepository = userLogRepository;
        }

        /// <summary>
        /// Deploys the image asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<bool> DeployImageAsync(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(DeployImageAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _algoBlobRepository.BlobExistsAsync(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoBinaryDataNotFound, "No blob for provided id");

                if (!await _algoMetaDataRepository.ExistsAlgoMetaDataAsync(data.AlgoClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo for provided id");

                if (data.AlgoClientId != data.ClientId && !await _publicAlgosRepository.ExistsPublicAlgoAsync(data.AlgoClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotPublic, $"Algo {data.AlgoId} not public for client {data.ClientId}");

                var instanceData = await _algoInstanceRepository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId);
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"No instance data for algo id {data.AlgoId}");

                if (!instanceData.ValidateData(out var instanceException))
                    throw instanceException;

                string blobKey = data.AlgoId + _algoBlobRepository.SourceExtension;
                var headers = _storageConnectionManager.GetData(blobKey);

                dynamic algoInstanceParameters = new ExpandoObject();
                algoInstanceParameters.AlgoId = data.AlgoId;
                algoInstanceParameters.InstanceId = data.InstanceId;

                var buildData = new TeamCityClientBuildData
                {
                    BlobAuthorizationHeader = headers.AuthorizationHeader,
                    BlobUrl = headers.Url,
                    BlobVersionHeader = headers.VersionHeader,
                    BlobDateHeader = headers.DateHeader,
                    AlgoId = data.InstanceId,
                    TradedAsset = instanceData.TradedAsset,
                    AssetPair = instanceData.AssetPair,
                    Volume = instanceData.Volume,
                    Margin = instanceData.Margin,
                    HftApiKey = "Dummy HFT Key",
                    HftApiUrl = "Dummy HFT Url",
                    WalletApiKey = "Dummy Wallet Key",
                    AlgoInstanceParameters = JsonConvert.SerializeObject(algoInstanceParameters)
                };

                var response = await _teamCityClient.StartBuild(buildData);

                return response.GetBuildState() != BuildStates.Undefined;
            });
        }
        /// <summary>
        /// Starts the test image asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<string> StartTestImageAsync(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(StartTestImageAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                if (!await _algoMetaDataRepository.ExistsAlgoMetaDataAsync(data.AlgoClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(data.InstanceId);
                if (pods.IsNullOrEmptyCollection())
                    return AlgoInstanceStatus.Deploying.ToString();

                if (pods.Count > 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for algoId {algoId}");

                var pod = pods[0];
                if (pod == null)
                    return AlgoInstanceStatus.Deploying.ToString();

                return pod.Status.Phase.ToUpper();
            });
        }

        /// <summary>
        /// Stops the test image asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<string> StopTestImageAsync(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(StopTestImageAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                if (!await _algoMetaDataRepository.ExistsAlgoMetaDataAsync(data.AlgoClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                if (data.AlgoClientId != data.ClientId && !await _publicAlgosRepository.ExistsPublicAlgoAsync(data.AlgoClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotPublic, $"Algo {data.AlgoId} not public for client {data.ClientId}");

                var instanceData = await _algoInstanceRepository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId);
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"No instance data for algo id {data.AlgoId}");

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(data.InstanceId);
                if (pods.IsNullOrEmptyCollection())
                    return AlgoInstanceStatus.Deploying.ToString();

                if (pods.Count > 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for algoId {algoId}");

                var pod = pods[0];
                if (pod == null)
                    return AlgoInstanceStatus.Deploying.ToString();

                var result = await _kubernetesApiClient.DeleteAsync(data.InstanceId, pod);

                if (!result)
                    return pod.Status.Phase.ToUpper();

                instanceData.AlgoInstanceStatus = AlgoInstanceStatus.Stopped;
                await _algoInstanceRepository.SaveAlgoInstanceDataAsync(instanceData);

                return AlgoInstanceStatus.Stopped.ToString();
            });
        }

        /// <summary>
        /// Gets the test tail log asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<string[]> GetTestTailLogAsync(TailLogData data)
        {
            return await LogTimedInfoAsync(nameof(GetTestTailLogAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _algoInstanceRepository.ExistsAlgoInstanceDataWithAlgoIdAsync(data.AlgoId, data.InstanceId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"Instance data not found data for algo {data.AlgoId} and instanceId {data.InstanceId}");

                var userLogs = await _userLogRepository.GetEntries(data.Tail, data.InstanceId);
                return userLogs.Select(l => $"[{l.Date:yyyy-MM-dd HH:mm:ss}] {l.Message}").ToArray();
            });
        }
        /// <summary>
        /// Deletes the image asynchronous.
        /// </summary>
        /// <param name="instanceData">The instance data.</param>
        /// <returns></returns>
        public async Task DeleteImageAsync(AlgoClientInstanceData instanceData)
        {
            await LogTimedInfoAsync(nameof(DeleteImageAsync), instanceData?.ClientId, async () =>
            {
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"Bad instance data");

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(instanceData.InstanceId);
                if (pods.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.PodNotFound, $"Pod is not found for {instanceData.InstanceId}");
                var pod = pods[0];
                if (pod == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.PodNotFound, $"Pod is not found for {instanceData.InstanceId}");

                var result = await _kubernetesApiClient.DeleteAsync(instanceData.InstanceId, pod);

                if (!result)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        $"Cannot delete image id {instanceData.InstanceId} for algo id {instanceData.AlgoId}");

                await _algoInstanceRepository.DeleteAlgoInstanceDataAsync(instanceData);
            });
        }

        public async Task DeleteInstanceAsync(AlgoClientInstanceData instanceData)
        {
            await LogTimedInfoAsync(nameof(DeleteImageAsync), instanceData?.ClientId, async () =>
            {
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"Bad instance data");

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(instanceData.InstanceId);

                if (!pods.IsNullOrEmptyCollection() && pods[0] != null)
                {
                    var pod = pods[0];

                    var result = await _kubernetesApiClient.DeleteAsync(instanceData.InstanceId, pod);

                    if (!result)
                        throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                            $"Cannot delete image id {instanceData.InstanceId} for algo id {instanceData.AlgoId}");
                }

                await _algoInstanceRepository.DeleteAlgoInstanceDataAsync(instanceData);

                //REMARK: Two lines below are commented out until we reach final decision on deletion
                //await _userLogRepository.DeleteAllAsync(instanceData.InstanceId);
                //await _statisticsRepository.DeleteAllAsync(instanceData.InstanceId);
            });
        }
    }
}
