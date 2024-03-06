using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public class PushNotification
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; } = "Unread";

        public string OrganizationId { get; set; }

        public string ShortMessage { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
