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

                var blob = await _algoBlobRepository.GetBlobAsync(data.AlgoId);

                var response =
                    await _externalClient.BuildAlgoImageFromBinaryAsync(blob, data.ClientId, algoMetaData.AlgoId);

                var imageId = int.Parse(response);
                var testId = await _externalClient.CreateTestAlgoAsync(imageId, algoMetaData.AlgoId);
                if (testId < 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, "Error creating test");

                var runtimeData = new AlgoClientRuntimeData
                {
                    ClientId = data.ClientId,
                    AlgoId = data.AlgoId,
                    ImageId = testId,
                    BuildImageId = imageId
                };

                await _algoRuntimeDataRepository.SaveAlgoRuntimeDataAsync(runtimeData);

                return true;
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

                var status = await _externalClient.GetAlgoTestAdministrativeStatusAsync(runtimeData.ImageId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                    $"GetAlgoTestAdministrativeStatus Status: {status} for imageId {runtimeData.ImageId}");

                var statusResult = AlgoRuntimeStatuses.Unknown;
                switch (status)
                {
                    case ClientAlgoRuntimeStatuses.Created:
                    case ClientAlgoRuntimeStatuses.Paused:
                    case ClientAlgoRuntimeStatuses.Stopped:
                        if (await _externalClient.StartTestAlgoAsync(runtimeData.ImageId))
                            statusResult = AlgoRuntimeStatuses.Started;
                        break;
                    default:
                        statusResult = status.ToModel();
                        break;
                }

                return statusResult.ToUpperText();
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

                var status = await _externalClient.GetAlgoTestAdministrativeStatusAsync(runtimeData.ImageId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                    $"GetAlgoTestAdministrativeStatus Status: {status} for imageId {runtimeData.ImageId}");

                var statusResult = AlgoRuntimeStatuses.Unknown;
                switch (status)
                {
                    case ClientAlgoRuntimeStatuses.Running:
                        if (await _externalClient.StopTestAlgoAsync(runtimeData.ImageId))
                            statusResult = AlgoRuntimeStatuses.Stopped;
                        break;
                    default:
                        statusResult = status.ToModel();
                        break;
                }

                return statusResult.ToUpperText();
            });
        }
        public async Task<string> GetTestLogAsync(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(GetTestLogAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeDataAsync(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound, $"Bad runtime data for {algoId}");

                return await _externalClient.GetTestAlgoLogAsync(runtimeData.ImageId);
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

                return await _externalClient.GetTestAlgoTailLogAsync(runtimeData.ImageId, data.Tail);
            });
        }
        public async Task DeleteImageAsync(AlgoClientRuntimeData runtimeData)
        {
            await LogTimedInfoAsync(nameof(DeleteImageAsync), runtimeData?.ClientId, async () =>
            {
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound, $"Bad runtime data");

                var status = await _externalClient.GetAlgoTestAdministrativeStatusAsync(runtimeData.ImageId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName,
                    $"GetAlgoTestAdministrativeStatus Status: {status} for testId {runtimeData.ImageId}");

                var result = true;

                if (status == ClientAlgoRuntimeStatuses.NotFound)
                    return;

                if (status == ClientAlgoRuntimeStatuses.Paused ||
                    status == ClientAlgoRuntimeStatuses.Running)
                {
                    result = await _externalClient.StopTestAlgoAsync(runtimeData.ImageId);
                    if (result)
                        status = ClientAlgoRuntimeStatuses.Stopped;
                }

                if (result &&
                    (status == ClientAlgoRuntimeStatuses.Stopped ||
                     status == ClientAlgoRuntimeStatuses.Created))
                    result = await _externalClient.DeleteTestAlgoAsync(runtimeData.ImageId);

                if (result)
                    result = await _externalClient.DeleteAlgoAsync(runtimeData.BuildImageId);

                if (result)
                    result = await _algoRuntimeDataRepository.DeleteAlgoRuntimeDataAsync(
                        runtimeData.ClientId,
                        runtimeData.AlgoId);

                if (!result)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        $"Cannot delete image id {runtimeData.ImageId} for algo id {runtimeData.AlgoId}");
            });
        }
    }
}
