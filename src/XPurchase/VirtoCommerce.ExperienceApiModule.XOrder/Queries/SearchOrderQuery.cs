using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrderQuery : SearchQuery<SearchOrderResponse>
    {
        public string Filter { get; set; }
        public string Facet { get; set; }
        public string CultureName { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<StringGraphType>(nameof(Sort), "The sort expression");
            yield return Argument<StringGraphType>(nameof(Facet), "This parameter applies a facet to the query results");
            yield return Argument<StringGraphType>(nameof(Filter), "This parameter applies a filter to the query results");
            yield return Argument<StringGraphType>(nameof(CultureName), "Culture name (\"en-US\")");
        }

        public override void Map(IResolveFieldContext context)
        {
            base.Map(context);

            CultureName = context.GetArgument<string>(nameof(CultureName));
            Filter = context.GetArgument<string>(nameof(Filter));
            Facet = context.GetArgument<string>(nameof(Facet));
        }
    }
}
