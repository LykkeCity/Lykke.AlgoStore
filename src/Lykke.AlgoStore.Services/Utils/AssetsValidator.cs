using Lykke.Service.Assets.Client.Models;
using System;
using System.Net;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Utils;
using Microsoft.Rest;
using Lykke.AlgoStore.Services.Strings;

namespace Lykke.AlgoStore.Services.Utils
{
    public class AssetsValidator
    {
        public void ValidateAssetResponse(Asset assetResponse)
        {
            if (assetResponse == null)
                throw new AlgoStoreException(AlgoStoreErrorCodes.NotFound, "Asset was not found",
                    string.Format(Phrases.ParamNotFoundDisplayMessage, "asset"));
        }

        public void ValidateAssetPair(string assetPairId, AssetPair assetPair)
        {
            if (assetPair == null)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.NotFound, $"AssetPair: {assetPairId} was not found",
                    string.Format(Phrases.ParamNotFoundDisplayMessage, "asset pair"));
            }
            if (assetPair.IsDisabled)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, $"AssetPair {assetPairId} is temporarily disabled",
                    string.Format(Phrases.AssetPairDisabledDisplayMessage, assetPair.Name));
            }
        }

        public void ValidateVolume(double volume, double minVolume, string asset)
        {
            if (Math.Abs(volume) < double.Epsilon || Math.Abs(volume) < minVolume)
            {
                var errorMessage = string.Format(Phrases.TradeVolumeBelowMinimum, minVolume, asset);
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, errorMessage, errorMessage);
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

        public void ValidateAsset(AssetPair assetPair, string tradedAssetId,
            Asset baseAsset, Asset quotingAsset)
        {
            if (tradedAssetId != baseAsset.Id && tradedAssetId != baseAsset.Name && tradedAssetId != quotingAsset.Id 
                && tradedAssetId != quotingAsset.Name)
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, 
                    $"Asset <{tradedAssetId}> is not valid for asset pair <{assetPair.Id}>.",
                    string.Format(Phrases.AssetInvalidForAssetPair, tradedAssetId, assetPair.Id));
            }
        }

        public void ValidateAccuracy(double volume, int accuracy)
        {
            if (volume.GetAccuracy() > accuracy)
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, "Volume accuracy is not valid for this Asset",
                    string.Format(Phrases.ParamInvalid, "volume accuracy"));
        }
    }
}
