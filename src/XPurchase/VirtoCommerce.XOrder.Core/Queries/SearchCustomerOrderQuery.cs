using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;

namespace VirtoCommerce.XOrder.Core.Queries
{
    public class SearchCustomerOrderQuery : SearchOrderQuery
    {
        public string CustomerId { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            foreach (var argument in base.GetArguments())
            {
                yield return argument;
            }

            yield return Argument<StringGraphType>("userId");
        }

        public override void Map(IResolveFieldContext context)
        {
            base.Map(context);

            CustomerId = context.GetArgument<string>("userId");
        }
    }
}
