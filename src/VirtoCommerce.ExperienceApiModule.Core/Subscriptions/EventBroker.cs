using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public class EventBroker
    {
        private readonly ISubject<PushNotification> _messageStream = new ReplaySubject<PushNotification>(1);

        public Task<IObservable<PushNotification>> MessagesAsync()
        {
            var observable = _messageStream.AsObservable();
            return Task.FromResult(observable);
        }

        public Task<IObservable<PushNotification>> MessagesByUserIdAsync(string userId)
        {
            var observable = _messageStream.AsObservable();

            if (!string.IsNullOrEmpty(userId))
            {
                observable = observable.Where(x => x.UserId == userId);
            }

            return Task.FromResult(observable);
        }

        public Task<PushNotification> AddMessageAsync(PushNotification message)
        {
            _messageStream.OnNext(message);

            return Task.FromResult(message);
        }
    }
}
