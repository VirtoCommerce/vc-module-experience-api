using System;
using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    [Obsolete("Use the same class from XCore.")]
    public class TermFacetResultType_Old : ObjectGraphType<TermFacetResult_Old>
    {
        public TermFacetResultType_Old()
        {
            Name = "TermFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field(d => d.Label, nullable: false).Description("Localized name of the facet.");
            Field<NonNullGraphType<FacetTypeEnum_Old>>("FacetType",
                "Three facet types: Terms, Range, and Filter");
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<FacetTermType_Old>>>>("Terms",
                "Terms",
                resolve: context => context.Source.Terms);

            IsTypeOf = obj => obj is TermFacetResult_Old;
            Interface<FacetInterface_Old>();
        }
    }
}
