using System;
using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    [Obsolete("Use the same class from XCore.")]
    public class RangeFacetResultType_Old : ObjectGraphType<RangeFacetResult_Old>
    {
        public RangeFacetResultType_Old()
        {
            Name = "RangeFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field(d => d.Label, nullable: false).Description("Localized name of the facet.");
            Field<NonNullGraphType<FacetTypeEnum_Old>>("FacetType",
                "The three types of facets. Terms, Range, Filter");
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<FacetRangeType_Old>>>>("Ranges",
                "Ranges",
                resolve: context => context.Source.Ranges);

            IsTypeOf = obj => obj is RangeFacetResult_Old;
            Interface<FacetInterface_Old>();
        }
    }
}
