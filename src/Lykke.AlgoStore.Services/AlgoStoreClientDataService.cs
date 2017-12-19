using System;
using System.Collections.Generic;
using System.IO;
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
    public class AlgoStoreClientDataService : BaseAlgoStoreService, IAlgoStoreClientDataService
    {
        private const string ComponentName = "AlgoStoreClientDataService";
        private readonly IAlgoMetaDataRepository _metaDataRepository;
        private readonly IAlgoRuntimeDataRepository _runtimeDataRepository;
        private readonly IAlgoBlobRepository _blobRepository;
        private readonly IDeploymentApiReadOnlyClient _deploymentClient;

        public AlgoStoreClientDataService(IAlgoMetaDataRepository metaDataRepository,
            IAlgoRuntimeDataRepository runtimeDataRepository,
            IAlgoBlobRepository blobRepository,
            IDeploymentApiReadOnlyClient deploymentClient,
            ILog log) : base(log)
        {
            _metaDataRepository = metaDataRepository;
            _runtimeDataRepository = runtimeDataRepository;
            _blobRepository = blobRepository;
            _deploymentClient = deploymentClient;
        }

        public async Task SaveAlgoAsBinary(string clientId, UploadAlgoBinaryData dataModel)
        {
            try
            {
                if (!dataModel.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                var algo = await _metaDataRepository.GetAlgoMetaData(clientId, dataModel.AlgoId);
                if (algo == null || algo.AlgoMetaData.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"Specified algo id {dataModel.AlgoId} is not found! Cant save file for a non existing algo.");

                using (var stream = new MemoryStream())
                {
                    await dataModel.Data.CopyToAsync(stream);
                    await _blobRepository.SaveBlobAsync(dataModel.AlgoId, stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<AlgoClientMetaData> GetClientMetadata(string clientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, "ClientId Is empty");

                var algos = await _metaDataRepository.GetAllClientAlgoMetaData(clientId);

                if (algos == null || algos.AlgoMetaData.IsNullOrEmptyCollection())
                    return algos;

                foreach (var metadata in algos.AlgoMetaData)
                {
                    var runtimeData = await _runtimeDataRepository.GetAlgoRuntimeData(clientId, metadata.AlgoId);

                    if (runtimeData == null)
                    {
                        metadata.Status = AlgoRuntimeStatuses.Unknown.ToUpperText();
                        // TODO Skip?!?
                        continue;
                    }

                    var status = ClientAlgoRuntimeStatuses.NotFound;
                    try
                    {
                        status = await _deploymentClient.GetAlgoTestAdministrativeStatus(runtimeData.ImageId);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, ex).Wait();
                    }
                    metadata.Status = status.ToModel().ToUpperText();
                }

                return algos;
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<AlgoClientRuntimeData> ValidateCascadeDeleteClientMetadataRequest(string clientId, AlgoMetaData data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, "ClientId Is empty");

                if (!data.ValidateData(out var exception))
                    throw exception;

                string algoId = data.AlgoId;
                if (!await _metaDataRepository.ExistsAlgoMetaData(clientId, algoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"Algo metadata not found for {algoId}");

                return await _runtimeDataRepository.GetAlgoRuntimeData(clientId, algoId);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<AlgoClientMetaData> SaveClientMetadata(string clientId, AlgoMetaData data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, "ClientId Is empty");

                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    data.AlgoId = Guid.NewGuid().ToString();

                if (!data.ValidateData(out var exception))
                    throw exception;

                var clientData = new AlgoClientMetaData
                {
                    ClientId = clientId,
                    AlgoMetaData = new List<AlgoMetaData> { data }
                };
                await _metaDataRepository.SaveAlgoMetaData(clientData);

                var res = await _metaDataRepository.GetAlgoMetaData(clientId, data.AlgoId);
                if (res == null || res.AlgoMetaData.IsNullOrEmptyCollection())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Cannot save data for {clientId} id: {data.AlgoId}");

                return res;
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task DeleteMetadata(string clientId, AlgoMetaData data)
        {
            if (await _blobRepository.BlobExists(data.AlgoId))
                await _blobRepository.DeleteBlobAsync(data.AlgoId);

            var clientData = new AlgoClientMetaData
            {
                ClientId = clientId,
                AlgoMetaData = new List<AlgoMetaData> { data }
            };
            await _metaDataRepository.DeleteAlgoMetaData(clientData);
        }

        public async Task<bool> DeleteAlgoRuntimeData(string clientId, string imageId)
        {
            return await _runtimeDataRepository.DeleteAlgoRuntimeData(clientId, imageId);
        }
    }
}
