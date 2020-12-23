using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class FilterFacetResultType : ObjectGraphType<FilterFacetResult>
    {
        public FilterFacetResultType()
        {
            Name = "FilterFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field<FacetTypeEnum>("FacetType", "The three types of facets. Terms, Range, Filter");
            Field(d => d.Count, nullable: false).Description("The number of products matching the value specified in the filter facet expression");
            Field(d => d.Label);

            IsTypeOf = obj => obj is FilterFacetResult;
            Interface<FacetInterface>();
        }
    }
}
