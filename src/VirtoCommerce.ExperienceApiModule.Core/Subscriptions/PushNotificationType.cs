using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public class PushNotificationType : ObjectGraphType<PushNotification>
    {
        public PushNotificationType()
        {
            Field(x => x.Id, nullable: true);
            Field(x => x.OrganizationId, nullable: true);
            Field(x => x.UserId, nullable: true);
            Field(x => x.Content, nullable: true);
            Field(x => x.SentDate, nullable: true);
        }
    }
}
