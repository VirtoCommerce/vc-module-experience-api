using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Queries
{
    public class GetPushMessagesQuery : Query<ExpPushMessagesResponse>
    {
        public bool UnreadOnly { get; set; }

        public string CultureName { get; set; }

        public string UserId { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<BooleanGraphType>(nameof(UnreadOnly));
            yield return Argument<StringGraphType>(nameof(CultureName));
        }

        public override void Map(IResolveFieldContext context)
        {
            UnreadOnly = context.GetArgument<bool>(nameof(UnreadOnly));
            CultureName = context.GetArgument<string>(nameof(CultureName));
        }
    }
}
