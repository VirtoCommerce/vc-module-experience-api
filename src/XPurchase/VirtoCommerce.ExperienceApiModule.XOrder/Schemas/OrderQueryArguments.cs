using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderQueryArguments : ArgumentList
    {
        public OrderQueryArguments()
        {
            Argument<StringGraphType>("id");
            Argument<StringGraphType>("number");
            Argument<StringGraphType>("cultureName", "Culture name (\"en-US\")");
        }
    }
}
