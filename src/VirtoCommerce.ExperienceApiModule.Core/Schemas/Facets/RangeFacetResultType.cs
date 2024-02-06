using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.Facets
{
    public class RangeFacetResultType : ObjectGraphType<RangeFacetResult>
    {
        public RangeFacetResultType()
        {
            Name = "RangeFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field(d => d.Label, nullable: false).Description("Localized name of the facet.");
            Field<NonNullGraphType<FacetTypeEnum>>("FacetType",
                "The three types of facets. Terms, Range, Filter");
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<FacetRangeType>>>>("Ranges",
                "Ranges",
                resolve: context => context.Source.Ranges);

            IsTypeOf = obj => obj is RangeFacetResult;
            Interface<FacetInterface>();
        }
    }
}
