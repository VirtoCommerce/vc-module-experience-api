using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public interface IEventBroker
    {
        Task<PushMessage> AddMessageAsync(PushMessage message);
        Task<IObservable<PushMessage>> MessagesAsync();
    }

    public class RedisPushMessage
    {
        public PushMessage Message { get; set; }
    }

    public class RedisEventBroker : IEventBroker
    {
        private readonly EventBroker _eventBroker;

        private const string ChannelName = "EventBroker";

        private static readonly RedisChannel _reddisChannel = RedisChannel.Literal(ChannelName);

        private readonly object _lock = new object();

        private bool _isSubscribed;

        private readonly IConnectionMultiplexer _connection;
        private readonly ISubscriber _subscriber;

        public RedisEventBroker(IConnectionMultiplexer connection, ISubscriber subscriber, EventBroker eventBroker)
        {
            _connection = connection;
            _subscriber = subscriber;
            _eventBroker = eventBroker;
        }

        public async Task<PushMessage> AddMessageAsync(PushMessage message)
        {
            EnsureRedisServerConnection();

            var redisPushMessage = new RedisPushMessage
            {
                Message = message
            };

            await _subscriber.PublishAsync(_reddisChannel, JsonConvert.SerializeObject(redisPushMessage), CommandFlags.FireAndForget);

            return message;
        }

        public Task<IObservable<PushMessage>> MessagesAsync()
        {
            return _eventBroker.MessagesAsync();
        }

        protected virtual void OnMessage(RedisChannel channel, RedisValue redisValue)
        {
            var message = JsonConvert.DeserializeObject<RedisPushMessage>(redisValue);

            if (message?.Message != null)
            {
                _eventBroker.AddMessageAsync(message.Message);
            }
        }

        private void EnsureRedisServerConnection()
        {
            if (!_isSubscribed)
            {
                lock (_lock)
                {
                    if (!_isSubscribed)
                    {
                        _subscriber.Subscribe(_reddisChannel, OnMessage, CommandFlags.FireAndForget);

                        _isSubscribed = true;
                    }
                }
            }

        }

    }

    public class EventBroker : IEventBroker
    {
        private readonly ISubject<PushMessage> _messageStream = new ReplaySubject<PushMessage>(0);

        public Task<IObservable<PushMessage>> MessagesAsync()
        {
            var observable = _messageStream.AsObservable();
            return Task.FromResult(observable);
        }

        public Task<IObservable<PushMessage>> MessagesByUserIdAsync(string userId)
        {
            var observable = _messageStream.AsObservable();

            if (!string.IsNullOrEmpty(userId))
            {
                observable = observable.Where(x => x.UserId == userId);
            }

            return Task.FromResult(observable);
        }

        public Task<PushMessage> AddMessageAsync(PushMessage message)
        {
            _messageStream.OnNext(message);

            return Task.FromResult(message);
        }
    }
}
