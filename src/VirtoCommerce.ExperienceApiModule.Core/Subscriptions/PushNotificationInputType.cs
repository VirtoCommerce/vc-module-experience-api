using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public class PushNotificationInputType : InputObjectGraphType
    {
        public PushNotificationInputType()
        {
            Field<StringGraphType>("content");
        }
    }
}
