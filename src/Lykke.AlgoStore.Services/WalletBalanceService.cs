using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Services.Strings;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.RateCalculator.Client;
using AssetPair = Lykke.Service.Assets.Client.Models.AssetPair;


namespace Lykke.AlgoStore.Services
{
    public class WalletBalanceService : BaseAlgoStoreService, IWalletBalanceService
    {
        private readonly IRateCalculatorClient _rateCalculator;
        private readonly IBalancesClient _balancesClient;

        public WalletBalanceService(IRateCalculatorClient rateCalculator, IBalancesClient balancesClient, ILog log)
            : base(log, nameof(WalletBalanceService))
        {
            _rateCalculator = rateCalculator;
            _balancesClient = balancesClient;
        }

        public void ValidateWallet(string walletId, AssetPair assetPair)
        {
            Task<IEnumerable<ClientBalanceResponseModel>> clientBalances = _balancesClient.GetClientBalances(walletId);

            var clientBalanceResponseModels = clientBalances.Result.ToList();
            var walletAssetIds = clientBalanceResponseModels.Select(cb => cb.AssetId).ToList();
            if (!clientBalanceResponseModels.Any())
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                    $"The wallet {walletId} has no assets in it.",
                    Phrases.WalletHasNoAssetsDisplayMessage);
            }

            if (!walletAssetIds.Contains(assetPair.BaseAssetId) && !walletAssetIds.Contains(assetPair.QuotingAssetId))
            {
                var errorMessage = string.Format(Phrases.AssetsMissingFromWallet, assetPair.BaseAssetId, assetPair.QuotingAssetId, walletId);
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, errorMessage, errorMessage);
            }
        }

        public async Task<IEnumerable<ClientBalanceResponseModel>> GetWalletBalancesAsync(string walletId, AssetPair assetPair)
        {
            return await LogTimedInfoAsync(nameof(GetWalletBalancesAsync), null, async () =>
            {               
                IEnumerable<ClientBalanceResponseModel> clientBalances =
                    await _balancesClient.GetClientBalances(walletId);

                var clientBalanceResponseModels = clientBalances.ToList();
                
                return clientBalanceResponseModels.Where(b => b.AssetId == assetPair.BaseAssetId || b.AssetId == assetPair.QuotingAssetId);
            });
        }


        public async Task<double> GetTotalWalletBalanceInBaseAssetAsync(string walletId, string baseAssetId, AssetPair assetPair)
        {
            return await LogTimedInfoAsync(nameof(GetTotalWalletBalanceInBaseAssetAsync), null, async () =>
            {
                double totalWalletBalance = 0;

                var balances = await GetWalletBalancesAsync(walletId, assetPair);
                var clientBalanceResponseModels = balances.ToList();

                foreach (var balance in clientBalanceResponseModels)
                {
                    if (balance.AssetId == baseAssetId)
                        totalWalletBalance += balance.Balance;
                    else
                    {
                        var assetBalanceInBase = await _rateCalculator.GetAmountInBaseAsync(balance.AssetId, balance.Balance, baseAssetId);
                        totalWalletBalance += assetBalanceInBase;
                    }
                }

                if (totalWalletBalance == 0)
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InitialWalletBalanceNotCalculated,
                        $"Initial wallet balance could not be calculated for wallet {walletId}");
                }

                return totalWalletBalance;
            });
        }

    }
}
