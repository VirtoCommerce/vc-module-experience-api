using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.Facets
{
    public class FilterFacetResultType : ObjectGraphType<FilterFacetResult>
    {
        public FilterFacetResultType()
        {
            Name = "FilterFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field(d => d.Label, nullable: false).Description("Localized name of the facet.");
            Field<NonNullGraphType<FacetTypeEnum>>("FacetType",
                "The three types of facets. Terms, Range, Filter");
            Field(d => d.Count, nullable: false).Description("The number of products matching the value specified in the filter facet expression");

            IsTypeOf = obj => obj is FilterFacetResult;
            Interface<FacetInterface>();
        }
    }
}
