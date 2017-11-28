using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoRuntimeDataMapper
    {
        public static AlgoClientRuntimeData ToModel(this IEnumerable<AlgoRuntimeDataEntity> entities)
        {
            var result = new AlgoClientRuntimeData { RuntimeData = new List<AlgoRuntimeData>() };

            if ((entities == null) || entities.IsNullOrEmptyEnumerable())
                return result;

            foreach (var algoEntity in entities)
            {
                if (algoEntity == null)
                    continue;

                result.ClientAlgoId = algoEntity.ClientAlgoId;
                result.RuntimeData.Add(algoEntity.ToRuntimeData());
            }

            return result;
        }
        public static List<AlgoRuntimeDataEntity> ToEntity(this AlgoClientRuntimeData data, string partitionKey)
        {
            var result = new List<AlgoRuntimeDataEntity>();

            if ((data == null) || data.RuntimeData.IsNullOrEmptyCollection())
                return result;

            var algoId = data.ClientAlgoId;

            foreach (AlgoRuntimeData runtimeData in data.RuntimeData)
            {
                if ((runtimeData == null) || (runtimeData.Asset == null) || (runtimeData.TradingAmount == null))
                    continue;

                var res = new AlgoRuntimeDataEntity();
                res.PartitionKey = partitionKey;
                res.ClientAlgoId = algoId;

                res.RowKey = runtimeData.ImageId;
                res.Version = runtimeData.Version;

                res.AssetAccuracy = runtimeData.Asset.Accuracy;
                res.AssetBaseAssetId = runtimeData.Asset.BaseAssetId;
                res.AssetId = runtimeData.Asset.Id;
                res.AssetInvertedAccuracy = runtimeData.Asset.InvertedAccuracy;
                res.AssetName = runtimeData.Asset.Name;
                res.AssetQuotingAssetId = runtimeData.Asset.QuotingAssetId;


                res.TradingAmountAmount = runtimeData.TradingAmount.Amount;
                res.TradingAmountAssetId = runtimeData.TradingAmount.AssetId;

                result.Add(res);
            }

            return result;
        }

        private static AlgoRuntimeData ToRuntimeData(this AlgoRuntimeDataEntity entity)
        {
            if (entity == null)
                return null;

            var result = new AlgoRuntimeData();

            result.ImageId = entity.RowKey;
            result.Asset = entity.ToTradingAssetData();
            result.TradingAmount = entity.ToTradingAmountData();
            result.Version = entity.Version;

            return result;
        }
        private static TradingAssetData ToTradingAssetData(this AlgoRuntimeDataEntity entity)
        {
            if (entity == null)
                return null;

            var result = new TradingAssetData();

            result.Accuracy = entity.AssetAccuracy;
            result.BaseAssetId = entity.AssetBaseAssetId;
            result.Id = entity.AssetId;
            result.InvertedAccuracy = entity.AssetAccuracy;
            result.Name = entity.AssetName;
            result.QuotingAssetId = entity.AssetQuotingAssetId;

            return result;
        }
        private static TradingAmountData ToTradingAmountData(this AlgoRuntimeDataEntity entity)
        {
            if (entity == null)
                return null;

            var result = new TradingAmountData();

            result.AssetId = entity.TradingAmountAssetId;
            result.Amount = entity.TradingAmountAmount;

            return result;
        }
    }
}
