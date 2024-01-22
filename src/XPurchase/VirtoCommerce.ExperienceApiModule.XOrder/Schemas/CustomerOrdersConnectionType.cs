using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using GraphQL.Types.Relay;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;
using VirtoCommerce.ExperienceApiModule.Core.Schemas.Facets;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class CustomerOrderConnectionType<TNodeType> : ConnectionType<TNodeType, EdgeType<TNodeType>>
        where TNodeType : IGraphType
    {
        public CustomerOrderConnectionType()
        {
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<TermFacetResultType>>>>("term_facets", description: "Term facets",
                resolve: context => ((CustomerOrderConnection<CustomerOrderAggregate>)context.Source).Facets.OfType<TermFacetResult>());

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<RangeFacetResultType>>>>("range_facets", description: "Range facets",
                resolve: context => ((ProductsConnection<ExpProduct>)context.Source).Facets.OfType<RangeFacetResult>());

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<FilterFacetResultType>>>>("filter_facets", description: "Filter facets",
                resolve: context => ((ProductsConnection<ExpProduct>)context.Source).Facets.OfType<FilterFacetResult>());
        }
    }

    public class CustomerOrderConnection<TNode> : PagedConnection<TNode>
    {
        public CustomerOrderConnection(IEnumerable<TNode> superset, int skip, int take, int totalCount)
            : base(superset, skip, take, totalCount)
        {
        }

        public IList<FacetResult> Facets { get; set; }
    }
}
