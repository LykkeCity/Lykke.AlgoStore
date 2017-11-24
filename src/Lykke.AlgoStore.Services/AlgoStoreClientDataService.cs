using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreClientDataService : IAlgoStoreClientDataService
    {
        private const string ComponentName = "AlgoStoreClientDataService";
        private readonly IAlgoMetaDataRepository _metaDataRepository;
        private readonly IAlgoDataRepository _dataRepository;
        private readonly IAlgoRuntimeDataRepository _runtimeDataRepository;
        private readonly ILog _log;

        public AlgoStoreClientDataService(IAlgoMetaDataRepository metaDataRepository,
            IAlgoDataRepository dataRepository,
            IAlgoRuntimeDataRepository runtimeDataRepository,
            ILog log)
        {
            _metaDataRepository = metaDataRepository;
            _dataRepository = dataRepository;
            _runtimeDataRepository = runtimeDataRepository;
            _log = log;
        }

        public async Task<BaseDataServiceResult<AlgoClientMetaData>> GetClientMetadata(string clientId)
        {
            var result = new BaseDataServiceResult<AlgoClientMetaData>();

            try
            {
                result.Data = await _metaDataRepository.GetAllClientMetaData(clientId);
            }
            catch (Exception ex)
            {
                _log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, ex).Wait();
                result.ResultError.ErrorCode = AlgoStoreErrorCodes.Unhandled;
            }

            return result;
        }

        public async Task<BaseServiceResult> CascadeDeleteClientMetadata(string clientId, AlgoMetaData data)
        {
            var result = new BaseServiceResult();

            try
            {
                var runtimeData = await _runtimeDataRepository.GetAlgoRuntimeDataByAlgo(data.ClientAlgoId);
                if (runtimeData != null)
                    return BaseServiceResult.CreateFromError(AlgoStoreErrorCodes.RuntimeSettingsExists);

                await _dataRepository.DeleteAlgoData(data.ClientAlgoId);

                var clientData = new AlgoClientMetaData
                {
                    ClientId = clientId,
                    AlgoMetaData = new List<AlgoMetaData> { data }
                };
                await _metaDataRepository.DeleteClientMetaData(clientData);
            }
            catch (Exception ex)
            {
                _log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, ex).Wait();
                result.ResultError.ErrorCode = AlgoStoreErrorCodes.Unhandled;
            }

            return result;
        }

        public async Task<BaseDataServiceResult<AlgoClientMetaData>> SaveClientMetadata(string clientId, AlgoMetaData data)
        {
            var result = new BaseDataServiceResult<AlgoClientMetaData>();

            try
            {
                var id = Guid.NewGuid().ToString();

                if (string.IsNullOrWhiteSpace(data.ClientAlgoId))
                    data.ClientAlgoId = id;

                var clientData = new AlgoClientMetaData
                {
                    ClientId = clientId,
                    AlgoMetaData = new List<AlgoMetaData> { data }
                };
                await _metaDataRepository.SaveClientMetaData(clientData);

                result.Data = await _metaDataRepository.GetClientMetaData(id);
            }
            catch (Exception ex)
            {
                _log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, ex).Wait();
                result.ResultError.ErrorCode = AlgoStoreErrorCodes.Unhandled;
            }

            return result;
        }


    }
}
