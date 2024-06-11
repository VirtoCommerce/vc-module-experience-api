using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using GraphQL.Types.Relay;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;
using VirtoCommerce.XDigitalCatalog.Core.Models;
using CoreFacets = VirtoCommerce.ExperienceApiModule.Core.Schemas.Facets;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas
{
    public class ProductsConnectonType<TNodeType> : ConnectionType<TNodeType, EdgeType<TNodeType>>
        where TNodeType : IGraphType
    {
        public ProductsConnectonType()
        {
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<CoreFacets.FilterFacetResultType>>>>("filter_facets",
                "Filter facets",
                resolve: context => ((ProductsConnection<ExpProduct>)context.Source).Facets.OfType<FilterFacetResult>());
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<CoreFacets.RangeFacetResultType>>>>("range_facets",
                "Range facets",
                resolve: context => ((ProductsConnection<ExpProduct>)context.Source).Facets.OfType<RangeFacetResult>());
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<CoreFacets.TermFacetResultType>>>>("term_facets",
                "Term facets",
                resolve: context => ((ProductsConnection<ExpProduct>)context.Source).Facets.OfType<TermFacetResult>());
        }
    }

    public class ProductsConnection<TNode> : PagedConnection<TNode>
    {
        public ProductsConnection(IEnumerable<TNode> superset, int skip, int take, int totalCount)
            : base(superset, skip, take, totalCount)
        {
        }

        public IList<FacetResult> Facets { get; set; }
    }
}
