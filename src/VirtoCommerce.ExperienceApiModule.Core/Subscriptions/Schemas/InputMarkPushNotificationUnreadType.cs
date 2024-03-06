using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas
{
    public class InputMarkPushNotificationUnreadType : InputObjectGraphType
    {
        public InputMarkPushNotificationUnreadType()
        {
            Field<NonNullGraphType<StringGraphType>>("notificationId");
        }
    }
}
