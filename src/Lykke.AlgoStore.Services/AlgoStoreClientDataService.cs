using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Services.Validation;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreClientDataService : IAlgoStoreClientDataService
    {
        private const string ComponentName = "AlgoStoreClientDataService";
        private readonly IAlgoMetaDataRepository _metaDataRepository;
        private readonly IAlgoDataRepository _dataRepository;
        private readonly IAlgoRuntimeDataRepository _runtimeDataRepository;
        private readonly IAlgoTemplateDataRepository _templateDataRepository;
        private readonly ILog _log;

        public AlgoStoreClientDataService(IAlgoMetaDataRepository metaDataRepository,
            IAlgoDataRepository dataRepository,
            IAlgoRuntimeDataRepository runtimeDataRepository,
            IAlgoTemplateDataRepository templateDataRepository,
            ILog log)
        {
            _metaDataRepository = metaDataRepository;
            _dataRepository = dataRepository;
            _runtimeDataRepository = runtimeDataRepository;
            _templateDataRepository = templateDataRepository;
            _log = log;
        }

        public async Task<AlgoClientMetaData> GetClientMetadata(string clientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, "ClientId Is empty");

                return await _metaDataRepository.GetAllClientAlgoMetaData(clientId);
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
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

                var clientData = new AlgoClientMetaData
                {
                    ClientId = clientId,
                    AlgoMetaData = new List<AlgoMetaData> { data }
                };
                await _metaDataRepository.DeleteAlgoMetaData(clientData);
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
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

                return await _metaDataRepository.GetAlgoMetaData(id);
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        public async Task<List<AlgoTemplateData>> GetTemplate(string languageId)
        {
            try
            {
                return await _templateDataRepository.GetTemplatesByLanguage(languageId);
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        public async Task<AlgoData> GetSource(string clientAlgoId)
        {
            try
            {
                return await _dataRepository.GetAlgoData(clientAlgoId);
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        public async Task SaveSource(AlgoData data)
        {
            try
            {
                await _dataRepository.SaveAlgoData(data);
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
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
                throw HandleException(ex);
            }
        }

        private AlgoStoreException HandleException(Exception ex)
        {
            var exception = ex as AlgoStoreException;

            if (exception == null)
                exception = new AlgoStoreException(AlgoStoreErrorCodes.Unhandled, ex);

            var validationException = exception as AlgoStoreAggregateException;
            if (validationException != null)
                _log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, validationException.ToBaseException()).Wait();
            else
                _log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, exception).Wait();

            return exception;
        }
    }
}
