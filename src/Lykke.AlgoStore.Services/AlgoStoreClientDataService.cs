using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.DeploymentApiClient;
using Lykke.AlgoStore.Services.Utils;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreClientDataService : BaseAlgoStoreService, IAlgoStoreClientDataService
    {
        private const string ComponentName = "AlgoStoreClientDataService";
        private readonly IAlgoMetaDataRepository _metaDataRepository;
        private readonly IAlgoRuntimeDataReadOnlyRepository _runtimeDataRepository;
        private readonly IAlgoBlobRepository _blobRepository;
        private readonly IDeploymentApiReadOnlyClient _deploymentClient;
        private readonly ILog _log;

        public AlgoStoreClientDataService(IAlgoMetaDataRepository metaDataRepository,
            IAlgoRuntimeDataReadOnlyRepository runtimeDataRepository,
            IAlgoBlobRepository blobRepository,
            IDeploymentApiReadOnlyClient deploymentClient,
            ILog log) : base(log)
        {
            _metaDataRepository = metaDataRepository;
            _runtimeDataRepository = runtimeDataRepository;
            _blobRepository = blobRepository;
            _deploymentClient = deploymentClient;
            _log = log;
        }

        public async Task DeleteAlgoBlobBinaryAsync(string algoId)
        {
            try
            {
                await _blobRepository.DeleteBlobAsync(algoId);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task DeleteAlgoBlobStringAsync(string algoId)
        {
            try
            {
                await _blobRepository.DeleteBlobAsync(algoId);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task SaveAlgoAsString(string algoId, string data)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(algoId) || String.IsNullOrWhiteSpace(data))
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"Specified algo id and/or algo string are empty! ");
                }
                var algo = await _metaDataRepository.GetAlgoMetaData(algoId);
                if (algo == null)
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"Specified algo id {algoId} is not found! ");
                }
                await _blobRepository.SaveBlobAsync(algoId, data);

            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }
        public async Task SaveAlgoAsBinary(UploadAlgoBinaryData dataModel)
        {
            try
            {
                if (!dataModel.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                var algo = await _metaDataRepository.GetAlgoMetaData(dataModel.AlgoId);
                if (algo == null)
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"Specified algo id {dataModel.AlgoId} is not found! Cant save file for a non existing algo.");
                }

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
                    var runtimeData = await _runtimeDataRepository.GetAlgoRuntimeDataByAlgo(metadata.ClientAlgoId);
                    long imageId;
                    if (runtimeData == null || 
                        runtimeData.RuntimeData.IsNullOrEmptyCollection() ||
                        (imageId = runtimeData.RuntimeData[0].GetImageIdAsNumber()) < 1)
                    {
                        metadata.Status = AlgoRuntimeStatuses.Uknown.ToUpperText();
                        // TODO Skip?!?
                        continue;
                    }

                    var status = await _deploymentClient.GetAlgoTestStatus(imageId);
                    metadata.Status = status.ToModel().ToUpperText();
                }

                return algos;
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task CascadeDeleteClientMetadata(string clientId, AlgoMetaData data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, "ClientId Is empty");

                if (!data.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                if (!await _metaDataRepository.ExistsAlgoMetaData(data.ClientAlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, $"Algo metadata not found for {data.ClientAlgoId}");

                var runtimeData = await _runtimeDataRepository.GetAlgoRuntimeDataByAlgo(data.ClientAlgoId);
                if ((runtimeData != null) && !runtimeData.RuntimeData.IsNullOrEmptyCollection())
                {
                    // runtime data should be deleted when image is deleted with external client. So just throw
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Cannot delete data. Image {runtimeData.RuntimeData[0].ImageId} is still running");
                }

                if (await _blobRepository.BlobExists(data.ClientAlgoId))
                    await _blobRepository.DeleteBlobAsync(data.ClientAlgoId);

                var clientData = new AlgoClientMetaData
                {
                    ClientId = clientId,
                    AlgoMetaData = new List<AlgoMetaData> { data }
                };
                await _metaDataRepository.DeleteAlgoMetaData(clientData);
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

                var id = Guid.NewGuid().ToString();

                if (string.IsNullOrWhiteSpace(data.ClientAlgoId))
                    data.ClientAlgoId = id;

                if (!data.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                var clientData = new AlgoClientMetaData
                {
                    ClientId = clientId,
                    AlgoMetaData = new List<AlgoMetaData> { data }
                };
                await _metaDataRepository.SaveAlgoMetaData(clientData);

                var res = await _metaDataRepository.GetAlgoMetaData(id);
                if (res == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, $"Cannot save data for {clientId} id: {data.ClientAlgoId}");

                return res;
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }

        public async Task<AlgoClientRuntimeData> GetRuntimeData(string clientAlgoId)
        {
            try
            {
                return await _runtimeDataRepository.GetAlgoRuntimeDataByAlgo(clientAlgoId);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }
    }
}
