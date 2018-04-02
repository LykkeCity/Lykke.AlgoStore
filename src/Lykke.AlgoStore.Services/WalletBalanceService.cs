using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.RateCalculator.Client;
using Lykke.Service.RateCalculator.Client.AutorestClient.Models;
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
                    $"The wallet {walletId} has no assets in it.");
            }

            if (!walletAssetIds.Contains(assetPair.BaseAssetId) || !walletAssetIds.Contains(assetPair.QuotingAssetId))
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                    $"Asset {assetPair.BaseAssetId} or {assetPair.QuotingAssetId} are missing from wallet {walletId}");
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

                List<AssetWithAmount> assetsWithAmount = new List<AssetWithAmount>();
                foreach (var balance in balances)
                {
                    if (balance.AssetId == baseAssetId)
                        totalWalletBalance += balance.Balance;
                    else
                    {
                        assetsWithAmount.Add(new AssetWithAmount
                        {
                            Amount = balance.Balance,
                            AssetId = balance.AssetId
                        });
                    }
                }

                var result = await _rateCalculator.GetMarketAmountInBaseAsync(assetsWithAmount, baseAssetId,
                    OrderAction.Sell);

                foreach (var resultBalance in result)
                {
                    if (resultBalance.Result == OperationResult.Ok)
                        totalWalletBalance += resultBalance.To.Amount;
                    else
                        throw new AlgoStoreException(AlgoStoreErrorCodes.InitialWalletBalanceNotCalculated,
                            $"There was a problem calculating {resultBalance.FromProperty.AssetId} value in {baseAssetId}. Error: {resultBalance.Result}");
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
