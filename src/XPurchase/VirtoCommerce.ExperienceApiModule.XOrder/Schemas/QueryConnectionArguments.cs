using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class QueryConnectionArguments : ArgumentList
    {
        public QueryConnectionArguments()
        {
            Argument<StringGraphType>("filter", "This parameter applies a filter to the query results");
            Argument<StringGraphType>("sort", "The sort expression");
            Argument<StringGraphType>("cultureName", "Culture name (\"en-US\")");
            Argument<StringGraphType>("userId", "");
        }

        public virtual QueryConnectionArguments AddArguments(QueryArguments arguments)
        {
            foreach (var argument in arguments)
                Add(argument);
            return this;
        }
    }
}
