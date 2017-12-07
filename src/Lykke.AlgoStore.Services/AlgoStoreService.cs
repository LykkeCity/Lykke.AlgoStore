using System;
using System.Collections.Generic;
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

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo for provided id");

                var algo = await _algoMetaDataRepository.GetAlgoMetaData(data.AlgoId);
                var algoMetaData = algo.AlgoMetaData?.FirstOrDefault();

                if (algoMetaData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo meta data for provided id");

                var blob = await _algoBlobRepository.GetBlobAsync(data.AlgoId);

                var response =
                    await _externalClient.BuildAlgoImageFromBinary(blob, data.ClientId, algoMetaData.ClientAlgoId);

                var runtimeData = new AlgoClientRuntimeData
                {
                    ClientAlgoId = data.AlgoId,
                    RuntimeData = new List<AlgoRuntimeData> { new AlgoRuntimeData { ImageId = response } }
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

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeDataByAlgo(algoId);
                if (runtimeData == null || runtimeData.RuntimeData.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound, $"No runtime data for algo id {algoId}");

                var imageId = runtimeData.RuntimeData[0].GetImageIdAsNumber();
                if (imageId < 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Image id is not long {algoId}");

                var status = await _externalClient.GetAlgoTestStatus(imageId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName, $"GetAlgoTestStatus Status: {status} for imageId {imageId}");

                var statusResult = AlgoRuntimeStatuses.Uknown;
                switch (status)
                {
                    case ClientAlgoRuntimeStatuses.NotFound:
                        if (await _externalClient.CreateTestAlgo(imageId, algoId) &&
                            await _externalClient.StartTestAlgo(imageId))
                            statusResult = AlgoRuntimeStatuses.Started;
                        break;
                    case ClientAlgoRuntimeStatuses.Created:
                    case ClientAlgoRuntimeStatuses.Paused:
                    case ClientAlgoRuntimeStatuses.Stopped:
                        if (await _externalClient.StartTestAlgo(imageId))
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

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"No algo for id {algoId}");

                var runtimeData = await _algoRuntimeDataRepository.GetAlgoRuntimeDataByAlgo(algoId);
                if (runtimeData == null || runtimeData.RuntimeData.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoRuntimeDataNotFound, $"No runtime data for algo id {algoId}");

                var imageId = runtimeData.RuntimeData[0].GetImageIdAsNumber();
                if (imageId < 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Image id is not long {algoId}");

                var status = await _externalClient.GetAlgoTestStatus(imageId);

                await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName, $"GetAlgoTestStatus Status: {status} for imageId {imageId}");

                var statusResult = AlgoRuntimeStatuses.Uknown;
                switch (status)
                {
                    case ClientAlgoRuntimeStatuses.Running:
                        if (await _externalClient.StopTestAlgo(imageId))
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
    }
}
