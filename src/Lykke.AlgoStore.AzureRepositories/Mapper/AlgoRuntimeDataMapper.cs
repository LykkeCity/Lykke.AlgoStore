using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoRuntimeDataMapper
    {
        public static AlgoClientRuntimeData ToModel(this IEnumerable<AlgoRuntimeEntity> entities)
        {
            var result = new AlgoClientRuntimeData { RuntimeData = new List<AlgoRuntimeData>() };

            if (entities.IsNullOrEmptyEnumerable())
                return result;

            var enumerator = entities.GetEnumerator();
            enumerator.Reset();
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                result.ClientId = current.PartitionKey;
                result.AlgoId = current.AlgoId;
                result.RuntimeData.Add(current.ToRuntimeData());
            }
            else
                return result;

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                result.RuntimeData.Add(current.ToRuntimeData());
            }

            return result;
        }
        public static List<AlgoRuntimeEntity> ToEntity(this AlgoClientRuntimeData data)
        {
            var result = new List<AlgoRuntimeEntity>();

            if (string.IsNullOrWhiteSpace(data.ClientId) || data.RuntimeData.IsNullOrEmptyCollection())
                return result;

            var clientId = data.ClientId;
            var algoId = data.AlgoId;

            foreach (AlgoRuntimeData runtimeData in data.RuntimeData)
            {
                if (runtimeData.Asset == null || runtimeData.TradingAmount == null)
                    continue;

                var res = new AlgoRuntimeEntity();
                res.PartitionKey = clientId;
                res.AlgoId = algoId;

                res.RowKey = runtimeData.ImageId;

                res.AssetAccuracy = runtimeData.Asset.Accuracy;
                res.AssetBaseAssetId = runtimeData.Asset.BaseAssetId;
                res.AssetId = runtimeData.Asset.Id;
                res.AssetInvertedAccuracy = runtimeData.Asset.InvertedAccuracy;
                res.AssetName = runtimeData.Asset.Name;
                res.AssetQuotingAssetId = runtimeData.Asset.QuotingAssetId;

                res.TemplateId = runtimeData.TemplateId;

                res.TradingAmountAmount = runtimeData.TradingAmount.Amount;
                res.TradingAmountAssetId = runtimeData.TradingAmount.AssetId;

                result.Add(res);
            }

            return result;
        }

        private static AlgoRuntimeData ToRuntimeData(this AlgoRuntimeEntity entity)
        {
            var result = new AlgoRuntimeData();

            result.ImageId = entity.RowKey;
            result.TemplateId = entity.TemplateId;
            result.Asset = entity.ToTradingAssetData();
            result.TradingAmount = entity.ToTradingAmountData();

            return result;
        }
        private static TradingAssetData ToTradingAssetData(this AlgoRuntimeEntity entity)
        {
            var result = new TradingAssetData();

            result.Accuracy = entity.AssetAccuracy;
            result.BaseAssetId = entity.AssetBaseAssetId;
            result.Id = entity.AssetId;
            result.InvertedAccuracy = entity.AssetAccuracy;
            result.Name = entity.AssetName;
            result.QuotingAssetId = entity.AssetQuotingAssetId;

            return result;
        }
        private static TradingAmountData ToTradingAmountData(this AlgoRuntimeEntity entity)
        {
            var result = new TradingAmountData();

            result.AssetId = entity.TradingAmountAssetId;
            result.Amount = entity.TradingAmountAmount;

            return result;
        }
    }
}
