using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas
{
    public class InputMarkPushMessageUnreadType : InputObjectGraphType
    {
        public InputMarkPushMessageUnreadType()
        {
            Field<NonNullGraphType<StringGraphType>>("notificationId");
        }
    }
}
