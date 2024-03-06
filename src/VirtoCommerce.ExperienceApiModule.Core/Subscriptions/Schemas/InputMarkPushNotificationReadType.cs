using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas
{
    public class InputMarkPushNotificationReadType : InputObjectGraphType
    {
        public InputMarkPushNotificationReadType()
        {
            Field<NonNullGraphType<StringGraphType>>("notificationId");
        }
    }
}
