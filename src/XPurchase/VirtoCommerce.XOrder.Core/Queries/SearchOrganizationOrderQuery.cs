using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;

namespace VirtoCommerce.XOrder.Core.Queries
{
    public class SearchOrganizationOrderQuery : SearchOrderQuery
    {
        public string OrganizationId { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            foreach (var argument in base.GetArguments())
            {
                yield return argument;
            }

            yield return Argument<StringGraphType>(nameof(OrganizationId));
        }

        public override void Map(IResolveFieldContext context)
        {
            base.Map(context);

            OrganizationId = context.GetArgument<string>(nameof(OrganizationId));
        }
    }
}
