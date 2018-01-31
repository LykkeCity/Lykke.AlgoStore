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
using Lykke.AlgoStore.Services.Utils;
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

        public AlgoStoreService(
            IDeploymentApiClient externalClient,
            ILog log,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IAlgoRuntimeDataRepository algoRuntimeDataRepository,
            IStorageConnectionManager storageConnectionManager,
            ITeamCityClient teamCityClient) : base(log, nameof(AlgoStoreService))
        {
            _externalClient = externalClient;
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
            _algoRuntimeDataRepository = algoRuntimeDataRepository;
            _storageConnectionManager = storageConnectionManager;
            _teamCityClient = teamCityClient;
        }

        public async Task<bool> DeployImage(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(DeployImage), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _algoBlobRepository.BlobExists(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoBinaryDataNotFound, "No blob for provided id");

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(data.ClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo for provided id");

                var algo = await _algoMetaDataRepository.GetAlgoMetaData(data.ClientId, data.AlgoId);
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

                await _algoRuntimeDataRepository.SaveAlgoRuntimeData(runtimeData);

                return response.GetBuildState() != BuildStates.Undefined;
            });
        }
        public async Task<string> StartTestImage(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(StartTestImage), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(data.ClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeData(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound,
                        $"No runtime data for algo id {algoId}");

                var buildStatus = await _teamCityClient.GetBuildStatus(runtimeData.BuildId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                    $"GetBuildStatus Status: {buildStatus.Status} {buildStatus.StatusText} for buildId {runtimeData.BuildId}");

                if (buildStatus.GetBuildStatus() != BuildStatuses.Success)
                    return buildStatus.Status.ToUpper();



                return string.Empty;

                //var status = await _externalClient.GetAlgoTestAdministrativeStatus(runtimeData.ImageId);

                //await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                //    $"GetAlgoTestAdministrativeStatus Status: {status} for imageId {runtimeData.ImageId}");

                //var statusResult = AlgoRuntimeStatuses.Unknown;
                //switch (status)
                //{
                //    case ClientAlgoRuntimeStatuses.Created:
                //    case ClientAlgoRuntimeStatuses.Paused:
                //    case ClientAlgoRuntimeStatuses.Stopped:
                //        if (await _externalClient.StartTestAlgo(runtimeData.ImageId))
                //            statusResult = AlgoRuntimeStatuses.Started;
                //        break;
                //    default:
                //        statusResult = status.ToModel();
                //        break;
                //}

                //return statusResult.ToUpperText();
            });
        }
        public async Task<string> StopTestImage(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(StopTestImage), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(data.ClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeData(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound,
                        $"No runtime data for algo id {algoId}");

                var status = await _externalClient.GetAlgoTestAdministrativeStatus(runtimeData.BuildId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                    $"GetAlgoTestAdministrativeStatus Status: {status} for imageId {runtimeData.BuildId}");

                var statusResult = AlgoRuntimeStatuses.Unknown;
                switch (status)
                {
                    case ClientAlgoRuntimeStatuses.Running:
                        if (await _externalClient.StopTestAlgo(runtimeData.BuildId))
                            statusResult = AlgoRuntimeStatuses.Stopped;
                        break;
                    default:
                        statusResult = status.ToModel();
                        break;
                }

                return statusResult.ToUpperText();
            });
        }
        public async Task<string> GetTestLog(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(GetTestLog), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeData(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Bad runtime data for {algoId}");

                return await _externalClient.GetTestAlgoLog(runtimeData.BuildId);
            });
        }
        public async Task<string> GetTestTailLog(TailLogData data)
        {
            return await LogTimedInfoAsync(nameof(GetTestTailLog), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeData(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Bad runtime data for {algoId}");

                return await _externalClient.GetTestAlgoTailLog(runtimeData.BuildId, data.Tail);
            });
        }
        public async Task DeleteImage(AlgoClientRuntimeData runtimeData)
        {
            await LogTimedInfoAsync(nameof(DeleteImage), runtimeData?.ClientId, async () =>
            {
                if (runtimeData == null)
                    return;

                var status = await _externalClient.GetAlgoTestAdministrativeStatus(runtimeData.BuildId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                    $"GetAlgoTestAdministrativeStatus Status: {status} for testId {runtimeData.BuildId}");

                var result = true;

                if (status == ClientAlgoRuntimeStatuses.NotFound)
                    return;

                if (status == ClientAlgoRuntimeStatuses.Paused ||
                    status == ClientAlgoRuntimeStatuses.Running)
                {
                    result = await _externalClient.StopTestAlgo(runtimeData.BuildId);
                    if (result)
                        status = ClientAlgoRuntimeStatuses.Stopped;
                }

                if (result &&
                    (status == ClientAlgoRuntimeStatuses.Stopped ||
                     status == ClientAlgoRuntimeStatuses.Created))
                    result = await _externalClient.DeleteTestAlgo(runtimeData.BuildId);

                if (result)
                    result = await _externalClient.DeleteAlgo(runtimeData.BuildId);

                if (result)
                    result = await _algoRuntimeDataRepository.DeleteAlgoRuntimeData(
                        runtimeData.ClientId,
                        runtimeData.AlgoId);

                if (!result)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        $"Cannot delete image id {runtimeData.BuildId} for algo id {runtimeData.AlgoId}");
            });
        }
    }
}
