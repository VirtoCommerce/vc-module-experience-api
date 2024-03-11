using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public class EventBroker
    {
        private readonly ISubject<PushMessage> _messageStream = new ReplaySubject<PushMessage>(1);

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
