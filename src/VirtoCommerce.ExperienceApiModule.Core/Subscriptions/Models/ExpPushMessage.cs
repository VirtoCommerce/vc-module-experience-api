using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models
{
    public class ExpPushMessage
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; } = "Unread";

        public string ShortMessage { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
