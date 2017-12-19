using System;
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

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreService : BaseAlgoStoreService, IAlgoStoreService
    {
        private const string ComponentName = "AlgoStoreService";

        private readonly IAlgoBlobReadOnlyRepository _algoBlobRepository;
        private readonly IAlgoMetaDataReadOnlyRepository _algoMetaDataRepository;
        private readonly IAlgoRuntimeDataRepository _algoRuntimeDataRepository;
        private readonly IDeploymentApiClient _externalClient;

        public AlgoStoreService(
            IDeploymentApiClient externalClient,
            ILog log,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IAlgoRuntimeDataRepository algoRuntimeDataRepository) : base(log)
        {
            _externalClient = externalClient;
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
            _algoRuntimeDataRepository = algoRuntimeDataRepository;
        }

        public async Task<bool> DeployImage(ManageImageData data)
        {
            try
            {
                if (!data.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                if (!await _algoBlobRepository.BlobExists(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoBinaryDataNotFound, "No blob for provided id");

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(data.ClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo for provided id");

                var algo = await _algoMetaDataRepository.GetAlgoMetaData(data.ClientId, data.AlgoId);
                var algoMetaData = algo.AlgoMetaData?.FirstOrDefault();

                if (algoMetaData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo meta data for provided id");

                var blob = await _algoBlobRepository.GetBlobAsync(data.AlgoId);

                var response =
                    await _externalClient.BuildAlgoImageFromBinary(blob, data.ClientId, algoMetaData.AlgoId);

                int imageId = int.Parse(response);
                var testId = await _externalClient.CreateTestAlgo(imageId, algoMetaData.AlgoId);
                if (testId < 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, "Error creating test");

                var runtimeData = new AlgoClientRuntimeData
                {
                    ClientId = data.ClientId,
                    AlgoId = data.AlgoId,
                    ImageId = testId,
                    BuildImageId = imageId
                };

                await _algoRuntimeDataRepository.SaveAlgoRuntimeData(runtimeData);

                return true;
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<string> StartTestImage(ManageImageData data)
        {
            try
            {
                if (!data.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                string algoId = data.AlgoId;

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(data.ClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeData(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound, $"No runtime data for algo id {algoId}");

                var status = await _externalClient.GetAlgoTestAdministrativeStatus(runtimeData.ImageId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName, $"GetAlgoTestAdministrativeStatus Status: {status} for imageId {runtimeData.ImageId}");

                var statusResult = AlgoRuntimeStatuses.Unknown;
                switch (status)
                {
                    case ClientAlgoRuntimeStatuses.Created:
                    case ClientAlgoRuntimeStatuses.Paused:
                    case ClientAlgoRuntimeStatuses.Stopped:
                        if (await _externalClient.StartTestAlgo(runtimeData.ImageId))
                            statusResult = AlgoRuntimeStatuses.Started;
                        break;
                    default:
                        statusResult = status.ToModel();
                        break;
                }

                return statusResult.ToUpperText();
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<string> StopTestImage(ManageImageData data)
        {
            try
            {
                if (!data.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                string algoId = data.AlgoId;

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(data.ClientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeData(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound, $"No runtime data for algo id {algoId}");

                var status = await _externalClient.GetAlgoTestAdministrativeStatus(runtimeData.ImageId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName, $"GetAlgoTestAdministrativeStatus Status: {status} for imageId {runtimeData.ImageId}");

                var statusResult = AlgoRuntimeStatuses.Unknown;
                switch (status)
                {
                    case ClientAlgoRuntimeStatuses.Running:
                        if (await _externalClient.StopTestAlgo(runtimeData.ImageId))
                            statusResult = AlgoRuntimeStatuses.Stopped;
                        break;
                    default:
                        statusResult = status.ToModel();
                        break;
                }

                return statusResult.ToUpperText();
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<string> GetTestLog(ManageImageData data)
        {
            try
            {
                if (!data.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                string algoId = data.AlgoId;

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeData(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Bad runtime data for {algoId}");

                return await _externalClient.GetTestAlgoLog(runtimeData.ImageId);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<string> GetTestTailLog(TailLogData data)
        {
            try
            {
                if (!data.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                string algoId = data.AlgoId;

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeData(data.ClientId, algoId);
                if (runtimeData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Bad runtime data for {algoId}");

                return await _externalClient.GetTestAlgoTailLog(runtimeData.ImageId, data.Tail);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<bool> DeleteImage(long imageId, int buildImageId)
        {
            var status = await _externalClient.GetAlgoTestAdministrativeStatus(imageId);

            await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName, $"GetAlgoTestAdministrativeStatus Status: {status} for testId {imageId}");

            var result = true;

            if (status == ClientAlgoRuntimeStatuses.NotFound)
                return true;

            if (status == ClientAlgoRuntimeStatuses.Paused ||
                status == ClientAlgoRuntimeStatuses.Running)
            {
                result = await _externalClient.StopTestAlgo(imageId);
                if (result)
                    status = ClientAlgoRuntimeStatuses.Stopped;
            }

            if (result &&
                (status == ClientAlgoRuntimeStatuses.Stopped ||
                 status == ClientAlgoRuntimeStatuses.Created))
                result = await _externalClient.DeleteTestAlgo(imageId);

            if (result)
                result = await _externalClient.DeleteAlgo(buildImageId);

            return result;
        }
    }
}
