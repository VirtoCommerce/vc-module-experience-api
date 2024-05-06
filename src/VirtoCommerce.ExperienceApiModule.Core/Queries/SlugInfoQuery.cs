using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SlugInfoQuery : Query<SlugInfoResponse>
    {
        public string Slug { get; set; }
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string CultureName { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<StringGraphType>(nameof(Slug));
            yield return Argument<StringGraphType>(nameof(StoreId));
            yield return Argument<StringGraphType>(nameof(UserId));
            yield return Argument<StringGraphType>(nameof(CultureName));
        }

        public override void Map(IResolveFieldContext context)
        {
            Slug = context.GetArgument<string>(nameof(Slug));
            StoreId = context.GetArgument<string>(nameof(StoreId));
            UserId = context.GetArgument<string>(nameof(UserId));
            CultureName = context.GetArgument<string>(nameof(CultureName));
        }
    }
}
