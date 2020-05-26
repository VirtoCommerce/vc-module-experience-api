using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class FacetInterface : InterfaceGraphType<FacetResult>
    {
        public FacetInterface()
        {
            Name = "Facet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field<FacetTypeEnum>("FacetType", "The three types of facets. Terms, Range, Filter");
        }
    }
}
