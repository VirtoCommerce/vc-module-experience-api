using System;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    // FOR TESTING PURPOSES ONLY
    public class PushNotificationJob(EventBroker eventBroker)
    {
        private readonly EventBroker _eventBroker = eventBroker;

        public async Task Process()
        {
            var message = new ExpPushMessage
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
