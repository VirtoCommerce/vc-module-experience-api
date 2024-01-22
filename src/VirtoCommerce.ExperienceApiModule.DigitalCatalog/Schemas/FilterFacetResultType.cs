using System;
using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    [Obsolete("Use the same class from XCore.")]
    public class FilterFacetResultType_Old : ObjectGraphType<FilterFacetResult_Old>
    {
        public FilterFacetResultType_Old()
        {
            Name = "FilterFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field(d => d.Label, nullable: false).Description("Localized name of the facet.");
            Field<NonNullGraphType<FacetTypeEnum_Old>>("FacetType",
                "The three types of facets. Terms, Range, Filter");
            Field(d => d.Count, nullable: false).Description("The number of products matching the value specified in the filter facet expression");

            IsTypeOf = obj => obj is FilterFacetResult_Old;
            Interface<FacetInterface_Old>();
        }
    }
}
