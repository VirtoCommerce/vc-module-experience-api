using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public class PushNotificationType : ObjectGraphType<PushNotification>
    {
        public PushNotificationType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.Status, nullable: true);
            Field(x => x.ShortMessage, nullable: true);
        }
    }
}
