using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models
{
    public class PushNotificationResponse
    {
        public int UnreadCount { get; set; }

        public List<PushNotification> Notifications { get; set; } = new List<PushNotification>();
    }
}
