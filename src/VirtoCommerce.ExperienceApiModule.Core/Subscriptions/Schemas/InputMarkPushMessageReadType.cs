using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas
{
    public class InputMarkPushMessageReadType : InputObjectGraphType
    {
        public InputMarkPushMessageReadType()
        {
            Field<NonNullGraphType<StringGraphType>>("notificationId");
        }
    }
}
