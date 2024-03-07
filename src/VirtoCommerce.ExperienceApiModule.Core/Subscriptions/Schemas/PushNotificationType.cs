using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas
{
    public class PushNotificationType : ObjectGraphType<PushNotification>
    {
        public PushNotificationType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.Status, nullable: true);
            Field(x => x.ShortMessage, nullable: true);
            Field(x => x.CreatedDate, nullable: true);
        }
    }
}