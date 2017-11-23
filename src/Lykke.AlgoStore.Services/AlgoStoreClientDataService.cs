using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreClientDataService : IAlgoStoreClientDataService
    {
        private readonly IAlgoClientMetaDataRepository _metaDataRepository;
        private readonly IAlgoDataRepository _algoDataRepository;

        public AlgoStoreClientDataService(IAlgoClientMetaDataRepository metaDataRepository, IAlgoDataRepository algoDataRepository)
        {
            _metaDataRepository = metaDataRepository;
            _algoDataRepository = algoDataRepository;
        }

        public async Task<AlgoClientMetaData> GetClientMetadata(string clientId)
        {
            return await _metaDataRepository.GetAllClientMetaData(clientId);
        }

        public async Task DeleteClientMetadata(string clientId, AlgoMetaData data)
        {
            await _algoDataRepository.DeleteAlgoData(clientId, data.Id);

            var clientData = new AlgoClientMetaData
            {
                ClientId = clientId,
                AlgosData = new List<AlgoMetaData> { data }
            };
            await _metaDataRepository.DeleteClientMetaData(clientData);
        }

        public async Task<AlgoClientMetaData> SaveClientMetadata(string clientId, AlgoMetaData data)
        {
            var id = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(data.Id))
                data.Id = id;

            var clientData = new AlgoClientMetaData
            {
                ClientId = clientId,
                AlgosData = new List<AlgoMetaData> { data }
            };
            await _metaDataRepository.SaveClientMetaData(clientData);

            return await _metaDataRepository.GetClientMetaData(clientId, id);
        }
    }
}
