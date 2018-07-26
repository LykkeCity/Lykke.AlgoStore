namespace Lykke.AlgoStore.Api.RealTimeStreaming.Filters
{
    public class DataFilter
    {
        public readonly string InstanceId;
        public readonly string AssetId;

        public DataFilter(string instanceId, string assetId)
        {
            InstanceId = instanceId;
            AssetId = assetId;
        }
        public DataFilter(string instanceId)
        {
            InstanceId = instanceId;
        }
    }
}
