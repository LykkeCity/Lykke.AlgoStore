using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.AlgoStore.TeamCityClient.Models;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreService : BaseAlgoStoreService, IAlgoStoreService
    {
        private readonly IAlgoMetaDataReadOnlyRepository _algoMetaDataRepository;
        private readonly IAlgoBlobReadOnlyRepository _algoBlobRepository;

        private readonly IAlgoRuntimeDataRepository _algoRuntimeDataRepository;
        private readonly IDeploymentApiClient _externalClient;

        private readonly IStorageConnectionManager _storageConnectionManager;
        private readonly ITeamCityClient _teamCityClient;

        private readonly IKubernetesApiClient _kubernetesApiClient;

        public AlgoStoreService(
            IDeploymentApiClient externalClient,
            ILog log,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IAlgoRuntimeDataRepository algoRuntimeDataRepository,
            IStorageConnectionManager storageConnectionManager,
            ITeamCityClient teamCityClient,
            IKubernetesApiClient kubernetesApiClient) : base(log, nameof(AlgoStoreService))
        {
            _externalClient = externalClient;
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
            _algoRuntimeDataRepository = algoRuntimeDataRepository;
            _storageConnectionManager = storageConnectionManager;
            _teamCityClient = teamCityClient;
            _kubernetesApiClient = kubernetesApiClient;
        }

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

                var algo = await _algoMetaDataRepository.GetAlgoMetaDataAsync(data.ClientId, data.AlgoId);
                var algoMetaData = algo.AlgoMetaData?.FirstOrDefault();

                if (algoMetaData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo meta data for provided id");

                var headers = _storageConnectionManager.GetData(data.AlgoId);

                var buildData = new TeamCityClientBuildData
                {
                    BlobAuthorizationHeader = headers.AuthorizationHeader,
                    BlobUrl = headers.Url,
                    BlobVersionHeader = headers.VersionHeader,
                    BlobDateHeader = headers.DateHeader,
                    AlgoId = data.AlgoId
                };

                var response = await _teamCityClient.StartBuild(buildData);

                var runtimeData = new AlgoClientRuntimeData
                {
                    ClientId = data.ClientId,
                    AlgoId = data.AlgoId,
                    BuildId = response.Id,
                    PodId = string.Empty
                };

                await _algoRuntimeDataRepository.SaveAlgoRuntimeDataAsync(runtimeData);

                return response.GetBuildState() != BuildStates.Undefined;
            });
        }
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

                return string.Empty;
                //var status = await _externalClient.GetAlgoTestAdministrativeStatusAsync(runtimeData.BuildId);

                //await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                //    $"GetAlgoTestAdministrativeStatus Status: {status} for imageId {runtimeData.BuildId}");

                //var statusResult = AlgoRuntimeStatuses.Unknown;
                //switch (status)
                //{
                //    case ClientAlgoRuntimeStatuses.Running:
                //        if (await _externalClient.StopTestAlgoAsync(runtimeData.BuildId))
                //            statusResult = AlgoRuntimeStatuses.Stopped;
                //        break;
                //    default:
                //        statusResult = status.ToModel();
                //        break;
                //}

                //return statusResult.ToUpperText();
            });
        }

        public async Task<string> GetTestTailLogAsync(TailLogData data)
        {
            return await LogTimedInfoAsync(nameof(GetTestTailLogAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeDataAsync(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound, $"Bad runtime data for {algoId}");

                var pods = await _kubernetesApiClient.ListPodsByAlgoIdAsync(algoId);
                if (pods.Count != 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for algoId {algoId}");

                var pod = pods[0];
                if (pod == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.PodNotFound, $"Pod for algoId {algoId} was not found");

                return await _kubernetesApiClient.ReadPodLogAsync(pod, data.Tail);
            });
        }
        public async Task DeleteImageAsync(AlgoClientRuntimeData runtimeData)
        {
            await LogTimedInfoAsync(nameof(DeleteImageAsync), runtimeData?.ClientId, async () =>
            {
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound, $"Bad runtime data");

                var status = await _externalClient.GetAlgoTestAdministrativeStatusAsync(runtimeData.BuildId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                    $"GetAlgoTestAdministrativeStatus Status: {status} for testId {runtimeData.BuildId}");

                var result = true;

                if (status == ClientAlgoRuntimeStatuses.NotFound)
                    return;

                if (status == ClientAlgoRuntimeStatuses.Paused ||
                    status == ClientAlgoRuntimeStatuses.Running)
                {
                    result = await _externalClient.StopTestAlgoAsync(runtimeData.BuildId);
                    if (result)
                        status = ClientAlgoRuntimeStatuses.Stopped;
                }

                if (result &&
                    (status == ClientAlgoRuntimeStatuses.Stopped ||
                     status == ClientAlgoRuntimeStatuses.Created))
                    result = await _externalClient.DeleteTestAlgoAsync(runtimeData.BuildId);

                if (result)
                    result = await _externalClient.DeleteAlgoAsync(runtimeData.BuildId);

                if (result)
                    result = await _algoRuntimeDataRepository.DeleteAlgoRuntimeDataAsync(
                        runtimeData.ClientId,
                        runtimeData.AlgoId);

                if (!result)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        $"Cannot delete image id {runtimeData.BuildId} for algo id {runtimeData.AlgoId}");
            });
        }
    }
}
