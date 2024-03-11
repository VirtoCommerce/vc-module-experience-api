using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas
{
    public class PushMessagesResponseType : ObjectGraphType<PushMessagesResponse>
    {
        public PushMessagesResponseType()
        {
            Field(x => x.UnreadCount, nullable: false);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<PushMessageType>>>>("items", resolve: x => x.Source.Items);
        }
    }
}
