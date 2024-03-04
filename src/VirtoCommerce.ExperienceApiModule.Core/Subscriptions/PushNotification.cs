using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public class PushNotification
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string OrganizationId { get; set; }

        public string Content { get; set; }

        public DateTime SentDate { get; set; }
    }
}
