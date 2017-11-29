using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreClientDataService : IAlgoStoreClientDataService
    {
        private const string ComponentName = "AlgoStoreClientDataService";
        private readonly IAlgoMetaDataRepository _metaDataRepository;
        private readonly IAlgoDataRepository _dataRepository;
        private readonly IAlgoRuntimeDataRepository _runtimeDataRepository;
        private readonly IAlgoTemplateDataRepository _templateDataRepository;
        private readonly IAlgoBaseRepository _blobRepository;
        private readonly ILog _log;

        public AlgoStoreClientDataService(IAlgoMetaDataRepository metaDataRepository,
            IAlgoDataRepository dataRepository,
            IAlgoRuntimeDataRepository runtimeDataRepository,
            IAlgoTemplateDataRepository templateDataRepository,
            IAlgoBaseRepository blobRepository,
            ILog log)
        {
            _metaDataRepository = metaDataRepository;
            _dataRepository = dataRepository;
            _runtimeDataRepository = runtimeDataRepository;
            _templateDataRepository = templateDataRepository;
            _blobRepository = blobRepository;
            _log = log;
        }

        public async Task SaveAlgoAsString(string key, string data)
        {
            await _blobRepository.SaveBlobAsStringAsync(key, data);
        }
        public async Task SaveAlgoAsBinary(string key, IFormFile data)
        {
            using (var stream = new MemoryStream())
            {
                await data.CopyToAsync(stream);
                await _blobRepository.SaveBlobAsByteArrayAsync(key, stream.ToArray());
            }
        }

        public async Task<AlgoClientMetaData> GetClientMetadata(string clientId)
        {
            try
            {
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
                var id = Guid.NewGuid().ToString();

                if (string.IsNullOrWhiteSpace(data.ClientAlgoId))
                    data.ClientAlgoId = id;

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

            _log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, exception).Wait();

            return exception;
        }
    }
}
