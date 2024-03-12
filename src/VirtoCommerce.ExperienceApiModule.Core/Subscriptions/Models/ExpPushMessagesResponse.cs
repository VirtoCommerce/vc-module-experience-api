using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models
{
    public class ExpPushMessagesResponse
    {
        public int UnreadCount { get; set; }

        public List<ExpPushMessage> Items { get; set; } = new List<ExpPushMessage>();
    }
}
