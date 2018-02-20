using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.AlgoStore.TeamCityClient.Models;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreService : BaseAlgoStoreService, IAlgoStoreService
    {
        private readonly IAlgoMetaDataReadOnlyRepository _algoMetaDataRepository;
        private readonly IAlgoBlobReadOnlyRepository _algoBlobRepository;
        private readonly IAlgoRuntimeDataRepository _algoRuntimeDataRepository; // TODO Should be removed
        private readonly IAlgoClientInstanceRepository _algoInstanceRepository;

        private readonly IStorageConnectionManager _storageConnectionManager;
        private readonly ITeamCityClient _teamCityClient;

        private readonly IKubernetesApiClient _kubernetesApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgoStoreService"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="algoBlobRepository">The algo BLOB repository.</param>
        /// <param name="algoMetaDataRepository">The algo meta data repository.</param>
        /// <param name="algoRuntimeDataRepository">The algo runtime data repository.</param>
        /// <param name="storageConnectionManager">The storage connection manager.</param>
        /// <param name="teamCityClient">The team city client.</param>
        /// <param name="kubernetesApiClient">The kubernetes API client.</param>
        /// <param name="algoInstanceRepository">The algo instance repository.</param>
        public AlgoStoreService(
            ILog log,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IAlgoRuntimeDataRepository algoRuntimeDataRepository,
            IStorageConnectionManager storageConnectionManager,
            ITeamCityClient teamCityClient,
            IKubernetesApiClient kubernetesApiClient,
            IAlgoClientInstanceRepository algoInstanceRepository) : base(log, nameof(AlgoStoreService))
        {
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
            _algoRuntimeDataRepository = algoRuntimeDataRepository;
            _storageConnectionManager = storageConnectionManager;
            _teamCityClient = teamCityClient;
            _kubernetesApiClient = kubernetesApiClient;
            _algoInstanceRepository = algoInstanceRepository;
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

                if (!await _algoMetaDataRepository.ExistsAlgoMetaDataAsync(data.ClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo for provided id");

                var instanceData = await _algoInstanceRepository.GetAlgoInstanceDataAsync(data.ClientId, data.AlgoId, data.InstanceId);
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"No instance data for algo id {data.AlgoId}");

                if (!instanceData.ValidateData(out var instanceException))
                    throw instanceException;

                string blobKey = data.AlgoId + _algoBlobRepository.SourceExtension;
                var headers = _storageConnectionManager.GetData(blobKey);

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
                    WalletApiKey = "Dummy Wallet Key"
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

                if (!await _algoMetaDataRepository.ExistsAlgoMetaDataAsync(data.ClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeDataAsync(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound,
                        $"No runtime data for algo id {algoId}");

                var buildStatus = await _teamCityClient.GetBuildStatus(runtimeData.BuildId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                    $"GetBuildStatus Status: {buildStatus.Status} {buildStatus.StatusText} for buildId {runtimeData.BuildId}");

                if (buildStatus.GetBuildStatus() != BuildStatuses.Success)
                    return buildStatus.Status.ToUpper();

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(algoId);
                if (pods == null)
                    return BuildStatuses.NotDeployed.ToUpperText();

                if (pods.Count != 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for algoId {algoId}");

                var pod = pods[0];
                if (pod == null)
                    return BuildStatuses.NotDeployed.ToUpperText();

                runtimeData.PodId = pod.Metadata.Name;
                await _algoRuntimeDataRepository.SaveAlgoRuntimeDataAsync(runtimeData);

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

                if (!await _algoMetaDataRepository.ExistsAlgoMetaDataAsync(data.ClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeDataAsync(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound,
                        $"No runtime data for algo id {algoId}");

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(algoId);
                if (pods.IsNullOrEmptyCollection())
                    return BuildStatuses.NotDeployed.ToUpperText();

                if (pods.Count != 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for algoId {algoId}");

                var pod = pods[0];
                if (pod == null)
                    return BuildStatuses.NotDeployed.ToUpperText();

                return pod.Status.Phase.ToUpper();
            });
        }

        /// <summary>
        /// Gets the test tail log asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<string> GetTestTailLogAsync(TailLogData data)
        {
            return await LogTimedInfoAsync(nameof(GetTestTailLogAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _algoInstanceRepository.ExistsAlgoInstanceDataAsync(data.ClientId, data.AlgoId, data.InstanceId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"Instance data not found data for algo {data.AlgoId} and instanceId {data.InstanceId}");

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(data.InstanceId);
                if (pods.Count != 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for instanceId {data.InstanceId}");

                var pod = pods[0];
                if (pod == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.PodNotFound, $"Pod for instanceId {data.InstanceId} was not found");

                return await _kubernetesApiClient.ReadPodLogAsync(pod, data.Tail);
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
