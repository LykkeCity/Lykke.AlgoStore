﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.DeploymentApiClient;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreService : BaseAlgoStoreService, IAlgoStoreService
    {
        private const string ComponentName = "AlgoStoreService";

        private readonly IApiDocumentation _deploymentApiClient;
        private readonly IAlgoBlobRepository<byte[]> _algoBlobRepository;
        private readonly IAlgoMetaDataRepository _algoMetaDataRepository;

        public AlgoStoreService(
            IApiDocumentation externalClient, 
            ILog log,
            IAlgoBlobRepository<byte[]> algoBlobRepository,
            IAlgoMetaDataRepository algoMetaDataRepository) : base(log)
        {
            _deploymentApiClient = externalClient;
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
        }

        public async Task<bool> DeployImage(DeployImageData data)
        {
            try
            {
                if (!data.ValidateData(out AlgoStoreAggregateException exception))
                    throw exception;

                var blob = await _algoBlobRepository.GetBlobAsync(data.AlgoId);
                var algo = await _algoMetaDataRepository.GetAlgoMetaData(data.AlgoId);
                var algoMetaData = algo?.AlgoMetaData.FirstOrDefault();
                
                //What now???
                //if (blob == null || algoMetaData == null)

                var stream = new MemoryStream(blob);

                var deployResponse = await _deploymentApiClient
                    .BuildAlgoImageFromBinaryUsingPOSTWithHttpMessagesAsync(stream, algoMetaData.ClientAlgoId,
                        algoMetaData.Name);

                //TODO: Check if we need to save response to AlgoRuntimeData for example

                return true;
            }
            catch (Exception ex)
            {
                throw HandleException(ex, ComponentName);
            }
        }
    }
}
