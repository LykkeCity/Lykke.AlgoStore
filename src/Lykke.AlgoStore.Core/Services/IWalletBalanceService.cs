using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IWalletBalanceService
    {
        Task<IEnumerable<ClientBalanceResponseModel>> GetWalletBalancesAsync(string walletId, AssetPair assetPair);
        Task<double> GetTotalWalletBalanceInBaseAssetAsync(string walletId, string baseAssetId, AssetPair assetPair);
        void ValidateWallet(string walletId, AssetPair assetPair);
    }
}
