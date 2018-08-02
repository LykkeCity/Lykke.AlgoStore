using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Sources.RabbitMq
{
    public class ObservableRabbitMqConnection<T> : RealTimeDataSourceBase<T> where T : IChartingUpdate//, new()
    {
        private readonly IObservable<T> _messages;
        private RabbitMqSubscriber<T> _rabbitMq;
        private readonly ILogFactory _logFactory; //https://github.com/LykkeCity/Lykke.Logs/blob/master/migration-to-v5.md
        private BlockingCollection<T> _messageQueue;
        private readonly RabbitMqSubscriptionSettings _rabbitSettings;
        private static object syncLock = new object();
        private readonly ILog Log;

        public ObservableRabbitMqConnection(RabbitMqSubscriptionSettings rabbitSettings, ILogFactory logFactory)
        {
            _messages = Observable.Create<T>(async (obs) => { await ReadRabbitMqMessagesLoop(obs); });
            _rabbitSettings = rabbitSettings;
            _logFactory = logFactory;
            Log = _logFactory.CreateLog(Constants.LogComponent);
        }

        private void EnsureRabbitMqIsInitialized()
        {
            if (_rabbitMq == null)
            {
                lock (syncLock)
                {
                    if (_rabbitMq == null)
                    {
                        if (String.IsNullOrWhiteSpace(ConnectionId))
                        {
                            throw new InvalidOperationException($"ConnectionId not set for ObservableRabbitMqConnection. Call {nameof(Configure)} first supplying unique connectionId.");
                        }
                        _rabbitSettings.QueueName = $"{_rabbitSettings.QueueName}{ConnectionId}";
                        _rabbitMq = new RabbitMqSubscriber<T>(_logFactory, _rabbitSettings, new DefaultErrorHandlingStrategy(_logFactory, _rabbitSettings))
                            .SetMessageDeserializer(new GenericRabbitModelConverter<T>())
                            .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                            .CreateDefaultBinding()
                            .Subscribe(OnMessageReceived);

                       Log.Info(nameof(EnsureRabbitMqIsInitialized), $"{typeof(T).Name} RabbitMq connection created:  {_rabbitSettings.QueueName}", "RabbitMqInit");
                    }
                }
            }
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
            EnsureRabbitMqIsInitialized();
            return _messages.Subscribe(observer);
        }

        private async Task ReadRabbitMqMessagesLoop(IObserver<T> obs)
        {
            await Task.Run(() =>
            {
                try
                {
                    _rabbitMq.Start();
                    Log.Info(nameof(ReadRabbitMqMessagesLoop), $"{typeof(T).Name} RabbitMq connection started:  {_rabbitSettings.QueueName}", "RabbitMqStarted");
                    _messageQueue = new BlockingCollection<T>();

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
                    _rabbitMq.Dispose();
                    _messageQueue.Dispose();
                    Log.Info(nameof(ReadRabbitMqMessagesLoop), $"{typeof(T).Name} RabbitMq connection closed:  {_rabbitSettings.QueueName}.", "RabbitMqClosed");
                }
            });
        }
    }
}
