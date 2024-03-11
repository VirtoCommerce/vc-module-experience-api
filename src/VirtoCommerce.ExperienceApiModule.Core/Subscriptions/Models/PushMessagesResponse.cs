using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models
{
    public class PushMessagesResponse
    {
        public int UnreadCount { get; set; }

        public List<PushMessage> Items { get; set; } = new List<PushMessage>();
    }
}
