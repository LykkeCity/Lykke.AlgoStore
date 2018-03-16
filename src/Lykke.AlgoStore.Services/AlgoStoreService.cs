using System;
using System.Dynamic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
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
        /// <param name="kubernetesApiClient">The kubernetes API client.</param>
        /// <param name="algoInstanceRepository">The algo instance repository.</param>
        public AlgoStoreService(
            ILog log,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IStorageConnectionManager storageConnectionManager,
            ITeamCityClient teamCityClient,
            IKubernetesApiClient kubernetesApiClient,
            IAlgoClientInstanceRepository algoInstanceRepository,
            IPublicAlgosRepository publicAlgosRepository) : base(log, nameof(AlgoStoreService))
        {
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
            _storageConnectionManager = storageConnectionManager;
            _teamCityClient = teamCityClient;
            _kubernetesApiClient = kubernetesApiClient;
            _algoInstanceRepository = algoInstanceRepository;
            _publicAlgosRepository = publicAlgosRepository;
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
                    return BuildStatuses.NotDeployed.ToUpperText();

                if (pods.Count > 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for algoId {algoId}");

                var pod = pods[0];
                if (pod == null)
                    return BuildStatuses.NotDeployed.ToUpperText();

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

                var instanceData = await _algoInstanceRepository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId);
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"No instance data for algo id {data.AlgoId}");

                if (!await _algoMetaDataRepository.ExistsAlgoMetaDataAsync(data.AlgoClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(data.InstanceId);
                if (pods.IsNullOrEmptyCollection())
                    return BuildStatuses.NotDeployed.ToUpperText();

                if (pods.Count > 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for algoId {algoId}");

                var pod = pods[0];
                if (pod == null)
                    return BuildStatuses.NotDeployed.ToUpperText();

                instanceData.AlgoInstanceStatus = CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Stopped;

                await _algoInstanceRepository.SaveAlgoInstanceDataAsync(instanceData);

                return pod.Status.Phase.ToUpper();
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

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(data.InstanceId);
                if (pods.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.PodNotFound, $"Pod for instanceId {data.InstanceId} was not found");

                if (pods.Count > 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for instanceId {data.InstanceId}");

                var pod = pods[0];
                if (pod == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.PodNotFound, $"Pod for instanceId {data.InstanceId} was not found");

                var result = await _kubernetesApiClient.ReadPodLogAsync(pod, data.Tail);

                // Remove last character from string to remove empty last line in log result
                var logArray = result?.Substring(0, result.Length - 1).Split('\n', StringSplitOptions.None) ?? new string[0];

                for(int i = 0; i < logArray.Length; i++)
                {
                    var currentLine = logArray[i];

                    var firstSpaceIndex = currentLine.IndexOf(' ');

                    var dateString = currentLine.Substring(0, firstSpaceIndex);
                    var restOfMessage = currentLine.Substring(firstSpaceIndex);

                    var date = DateTime.Parse(dateString).ToUniversalTime();

                    logArray[i] = $"[{date:yyyy-MM-dd HH:mm:ss}]{restOfMessage}";
                }

                return logArray;
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
                if (result)
                    await _algoInstanceRepository.DeleteAlgoInstanceDataAsync(instanceData);

                if (!result)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        $"Cannot delete image id {instanceData.InstanceId} for algo id {instanceData.AlgoId}");
            });
        }
    }
}
