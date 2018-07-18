using Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Sources.RabbitMq
{
    public class ObservableRabbitMqConnection<T> : RealTimeDataSourceBase<T> where T : BaseDataModel, new()
    {
        private readonly IObservable<T> _messages;
        private RabbitMqSubscriber<T> _subscriber;
        private readonly ILogFactory _logFactory; //https://github.com/LykkeCity/Lykke.Logs/blob/master/migration-to-v5.md
        private readonly BlockingCollection<T> _messageQueue;
        private readonly RabbitMqSubscriptionSettings _rabbitSettings;

        public ObservableRabbitMqConnection(RabbitMqSubscriptionSettings rabbitSettings, ILogFactory logFactory)
        {
            _messages = Observable.Create<T>(async (obs) => { await ReadRabbitMqMessagesLoop(obs); });
            _messageQueue = new BlockingCollection<T>();
            _rabbitSettings = rabbitSettings;
            _logFactory = logFactory;
            InitializeRabbitMqConnection();
        }

        private void InitializeRabbitMqConnection()
        {
            _subscriber = new RabbitMqSubscriber<T>(_logFactory, _rabbitSettings, new DefaultErrorHandlingStrategy(_logFactory, _rabbitSettings))
                .SetMessageDeserializer(new GenericRabbitModelConverter<T>())
                .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                .CreateDefaultBinding()
                .Subscribe(OnMessageReceived);
        }

        private Task OnMessageReceived(T message)
        {
            if (message.IsAllowedByFilter(Filter))
            {
                _messageQueue.TryAdd(message, TimeSpan.FromSeconds(5));
            }
            return Task.CompletedTask;
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            return _messages.Subscribe(observer);
        }

        private async Task ReadRabbitMqMessagesLoop(IObserver<T> obs)
        {
            await Task.Run(() =>
            {
                try
                {
                    _subscriber.Start();

                    while (!TokenSource.IsCancellationRequested && !_messageQueue.IsCompleted)
                    {
                        if (_messageQueue.TryTake(out T message, TimeSpan.FromSeconds(2)))
                        {
                            obs.OnNext(message);
                        }
                    }
                    obs.OnCompleted();
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    obs.OnError(ex);
                    return Task.FromException(ex);
                }
                finally
                {
                    _subscriber.Dispose();
                    _messageQueue.Dispose();
                }
            });
        }
    }
}
