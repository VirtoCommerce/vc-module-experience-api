using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using GraphQL.Types.Relay;
using GraphQL.Types.Relay.DataObjects;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class ProductsConnectonType<TNodeType> : ConnectionType<TNodeType, EdgeType<TNodeType>>
        where TNodeType : IGraphType
    {
        public ProductsConnectonType()
        {
            Field<ListGraphType<FilterFacetResultType>>("filter_facets",
               resolve: context => ((ProductsConnection<ExpProduct>)context.Source).Facets.OfType<FilterFacetResult>());

            Field<ListGraphType<RangeFacetResultType>>("range_facets",
               resolve: context => ((ProductsConnection<ExpProduct>)context.Source).Facets.OfType<RangeFacetResult>());

            Field<ListGraphType<TermFacetResultType>>("term_facets",
                resolve: context => ((ProductsConnection<ExpProduct>)context.Source).Facets.OfType<TermFacetResult>());
        }
    }

    public class ProductsConnection<TNode> : PagedConnection<TNode>
    {
        public ProductsConnection(IEnumerable<TNode> superset, int skip, int first, int totalCount)
            : base(superset, skip, first, totalCount)
        {
        }
        public IList<FacetResult> Facets { get; set; }
    }
}
