using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class FilterFacetResultType : ObjectGraphType<FilterFacetResult>
    {
        public FilterFacetResultType()
        {
            Name = "FilterFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field<FacetTypeEnum>("FacetType", "The three types of facets. Terms, Range, Filter");
            Field(d => d.Count, nullable: false).Description("The number of products matching the value specified in the filter facet expression");

            IsTypeOf = obj => obj is FilterFacetResult;
            Interface<FacetInterface>();
        }
    }
}
