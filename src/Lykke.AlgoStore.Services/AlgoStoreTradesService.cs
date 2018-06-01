using Common.Log;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.AlgoTrades.Client;
using Lykke.AlgoStore.Service.AlgoTrades.Client.AutorestClient.Models;
using Lykke.AlgoStore.Services.Strings;
using Lykke.AlgoStore.Services.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreTradesService : BaseAlgoStoreService, IAlgoStoreTradesService
    {
        private readonly IAlgoTradesClient _algoTradesClient;
        private readonly IAlgoClientInstanceRepository _algoInstanceRepository;
        private readonly int _maxNumberOfRowsToFetch;

        public AlgoStoreTradesService(
            ILog log,
            IAlgoTradesClient algoTradesClient,
            IAlgoClientInstanceRepository algoClientInstanceRepository,
            int maxNumberOfRowsToFetch) : base(log, nameof(AlgoStoreTradesService))
        {
            _algoTradesClient = algoTradesClient;
            _algoInstanceRepository = algoClientInstanceRepository;
            _maxNumberOfRowsToFetch = maxNumberOfRowsToFetch;
        }

        public async Task<IEnumerable<AlgoInstanceTradeResponseModel>> GetAllTradesForAlgoInstanceAsync(string clientId, string instanceId)
        {
            return await LogTimedInfoAsync(nameof(GetAllTradesForAlgoInstanceAsync), instanceId, async () =>
            {
                Check.IsEmpty(clientId, nameof(clientId));
                Check.IsEmpty(instanceId, nameof(instanceId));

                var algoInstance = await _algoInstanceRepository.GetAlgoInstanceDataByClientIdAsync(clientId, instanceId);
                if (algoInstance == null || algoInstance.InstanceId == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"Could not find AlgoInstance with InstanceId {instanceId} and and ClientId {clientId}", Phrases.InstanceNotFound);

                var result = await _algoTradesClient.GetAlgoInstanceTradesByTradedAsset(instanceId, algoInstance.TradedAsset, _maxNumberOfRowsToFetch);

                if (result.Error != null && !string.IsNullOrEmpty(result.Error.Message))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoTradesClientError, result.Error.Message);

                return result.Records;
            });
        }
    }
}
