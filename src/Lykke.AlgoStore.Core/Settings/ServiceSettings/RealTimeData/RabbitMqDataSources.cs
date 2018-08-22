namespace Lykke.AlgoStore.Core.Settings.ServiceSettings.RealTimeDataStreamingSettings
{
    public class RabbitMqDataSources
    {
        public RabbitMqConfig Dummy { get; set; }
        public RabbitMqConfig Candles { get; set; }
        public RabbitMqConfig Trades { get; set; }
        public RabbitMqConfig Functions { get; set; }
        public RabbitMqConfig Quotes { get; set; }
    }
}
