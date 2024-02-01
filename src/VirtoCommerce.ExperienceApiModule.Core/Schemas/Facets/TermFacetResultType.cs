using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.Facets
{
    public class TermFacetResultType : ObjectGraphType<TermFacetResult>
    {
        public TermFacetResultType()
        {
            Name = "TermFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field(d => d.Label, nullable: false).Description("Localized name of the facet.");
            Field<NonNullGraphType<FacetTypeEnum>>("FacetType",
                "Three facet types: Terms, Range, and Filter");
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<FacetTermType>>>>("Terms",
                "Terms",
                resolve: context => context.Source.Terms);

            IsTypeOf = obj => obj is TermFacetResult;
            Interface<FacetInterface>();
        }
    }
}
