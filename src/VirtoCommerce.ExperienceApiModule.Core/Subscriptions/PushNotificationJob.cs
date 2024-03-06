using System;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    // FOR TESTING PURPOSES ONLY
    public class PushNotificationJob(EventBroker eventBroker)
    {
        private readonly EventBroker _eventBroker = eventBroker;

        public async Task Process()
        {
            var message = new PushNotification
            {
                Id = Guid.NewGuid().ToString(),
                ShortMessage = "Scheduled event",
                CreatedDate = DateTime.UtcNow,
                UserId = "Hangfire",
            };

            await _eventBroker.AddMessageAsync(message);
        }
    }
}
