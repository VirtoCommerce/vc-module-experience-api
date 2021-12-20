using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderQueryConnectionArguments : ArgumentList
    {
        public OrderQueryConnectionArguments()
        {
            Argument<StringGraphType>("filter", "This parameter applies a filter to the query results");
            Argument<StringGraphType>("sort", "The sort expression");
            Argument<StringGraphType>("cultureName", "Culture name (\"en-US\")");
            Argument<StringGraphType>("userId", "");
        }

        public virtual OrderQueryConnectionArguments AddArguments(QueryArguments arguments)
        {
            foreach (var argument in arguments)
                Add(argument);
            return this;
        }
    }
}
