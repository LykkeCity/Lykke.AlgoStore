using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Sources.RabbitMq
{
    public class RabbitMqDataSource<T> : RealTimeDataSource<T> where T : IChartingUpdate
    {
        private RabbitMqSubscriber<T> _rabbitMq;
        private readonly ILogFactory _logFactory; //https://github.com/LykkeCity/Lykke.Logs/blob/master/migration-to-v5.md
        private readonly RabbitMqSubscriptionSettings _rabbitSettings;
        private readonly ILog Log;

        private readonly HashSet<Func<T, Task>> _subscriptions = new HashSet<Func<T, Task>>();

        public RabbitMqDataSource(RabbitMqSubscriptionSettings rabbitSettings, ILogFactory logFactory)
        {
            _rabbitSettings = rabbitSettings;
            _logFactory = logFactory;
            Log = _logFactory.CreateLog(Constants.LogComponent);
        }

        public override void Subscribe(Func<T, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            EnsureRabbitMqIsInitialized();

            if (!_subscriptions.Any())
                _rabbitMq.Start();

            _subscriptions.Add(handler);
        }

        public override void Unsubscribe(Func<T, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _subscriptions.Remove(handler);

            if (!_subscriptions.Any())
                _rabbitMq.Stop();
        }

        private void EnsureRabbitMqIsInitialized()
        {
            if (_rabbitMq != null) return;

            _rabbitMq = new RabbitMqSubscriber<T>(_logFactory, _rabbitSettings, new DefaultErrorHandlingStrategy(_logFactory, _rabbitSettings))
                .SetMessageDeserializer(new GenericRabbitModelConverter<T>())
                .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                .CreateDefaultBinding()
                .Subscribe(OnMessageReceived)
                .Start();

            Log.Info(nameof(EnsureRabbitMqIsInitialized), 
                $"{typeof(T).Name} RabbitMq connection created:  {_rabbitSettings.QueueName}", "RabbitMqInit");
        }

        private async Task OnMessageReceived(T message)
        {
            await Task.WhenAll(_subscriptions.ToArray().Select(h => h(message)));
        }
    }
}
