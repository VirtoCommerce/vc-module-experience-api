using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas
{
    public class PushNotificationsResponseType : ObjectGraphType<PushNotificationResponse>
    {
        public PushNotificationsResponseType()
        {
            Field(x => x.UnreadCount, nullable: false);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<PushNotificationType>>>>("items", resolve: x => x.Source.Notifications);
        }
    }
}
