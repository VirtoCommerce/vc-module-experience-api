using System;
using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    [Obsolete("Use the same class from XCore.")]
    public class FacetInterface : InterfaceGraphType<FacetResult>
    {
        public FacetInterface()
        {
            Name = "Facet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field(d => d.Label, nullable: false).Description("Localized name of the facet.");
            Field<NonNullGraphType<FacetTypeEnum>>("FacetType",
                "Three facet types: Terms, Range, and Filter");
        }
    }
}
