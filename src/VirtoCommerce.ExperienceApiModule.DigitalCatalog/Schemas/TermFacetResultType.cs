using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TermFacetResultType : ObjectGraphType<TermFacetResult>
    {
        public TermFacetResultType()
        {
            Name = "TermFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field(d => d.Label, nullable: false).Description("Localized name of the facet.");
            Field<FacetTypeEnum>("FacetType", "The three types of facets. Terms, Range, Filter");
            Field<ListGraphType<FacetTermType>>("Terms", "Terms", resolve: context => context.Source.Terms);

            IsTypeOf = obj => obj is TermFacetResult;
            Interface<FacetInterface>();
        }
    }
}
