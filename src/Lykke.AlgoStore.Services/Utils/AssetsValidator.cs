using Lykke.Service.Assets.Client.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Utils;
using Microsoft.Rest;

namespace Lykke.AlgoStore.Services.Utils
{
    public class AssetsValidator
    {
        public void ValidateAssetPairResponse(HttpOperationResponse<AssetPair> assetPairResponse)
        {
            if (assetPairResponse.Response.StatusCode != HttpStatusCode.OK)
                throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                    $"Invalid response code: {assetPairResponse.Response.StatusCode} from asset service calling AssetPairGetWithHttpMessagesAsync");
        }

        public void ValidateAssetResponse(HttpOperationResponse<Asset> assetResponse)
        {
            if (assetResponse.Response.StatusCode != HttpStatusCode.OK)
                throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                    $"Invalid response code: {assetResponse.Response.StatusCode} from asset service calling AssetGetWithHttpMessagesAsync");
        }

        public void ValidateAssetPair(string assetPairId, AssetPair assetPair)
        {
            if (assetPair == null)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.AssetNotFound, $"AssetPair: {assetPairId} was not found");
            }
            if (assetPair.IsDisabled)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"AssetPair {assetPairId} is temporarily disabled");              
            }
        }

        public void ValidateVolume(double volume, double minVolume, string asset)
        {
            if (Math.Abs(volume) < double.Epsilon || Math.Abs(volume) < minVolume)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"The amount should be higher than minimal order size {minVolume} {asset}");
            }
        }

        //REMARK - To be used for limit orders
        //public bool ValidatePrice(double price)
        //{
        //    if (Math.Abs(price) < double.Epsilon)
        //    {
        //        model = ResponseModel.CreateInvalidFieldError("Price", "Price must be greater than asset accuracy.");
        //        return false;
        //    }

        //    model = null;
        //    return true;
        //}

        public void ValidateAsset(AssetPair assetPair, string assetId,
            Asset baseAsset, Asset quotingAsset)
        {
            if (assetId != baseAsset.Id && assetId != baseAsset.Name && assetId != quotingAsset.Id && assetId != quotingAsset.Name)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"Asset <{assetId}> is not valid for asset pair <{assetPair.Id}>.");
            }
        }

        public void ValidateAccuracy(double volume, int accuracy)
        {
            if (volume.GetAccuracy() > accuracy)
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "Volume accuracy is not valid for this Asset");
        }
    }
}
