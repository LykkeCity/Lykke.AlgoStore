using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Core.Settings.ServiceSettings.RealTimeDataStreamingSettings
{
    public class RabbitMqConfig
    {
        public string ConnectionString { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
    }
}
