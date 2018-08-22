using Lykke.AlgoStore.Core.Settings.ServiceSettings.RealTimeDataStreamingSettings;

namespace Lykke.AlgoStore.Core.Settings.ServiceSettings.RealTimeData
{
    public class RealTimeDataSettings
    {
        public RabbitMqDataSources RabbitMqSources { get; set; }

        public uint MaxConnectionsPerClient { get; set; }
        public uint MaxInstancesPerClient { get; set; }
    }
}
