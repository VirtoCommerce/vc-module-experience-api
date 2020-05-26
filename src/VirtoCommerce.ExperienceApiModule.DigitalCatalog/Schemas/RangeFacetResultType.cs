using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class RangeFacetResultType : ObjectGraphType<RangeFacetResult>
    {
        public RangeFacetResultType()
        {
            Name = "RangeFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field<FacetTypeEnum>("FacetType", "The three types of facets. Terms, Range, Filter");
            Field<ListGraphType<FacetRangeType>>("Ranges", "Ranges", resolve: context => context.Source.Ranges);

            IsTypeOf = obj => obj is RangeFacetResult;
            Interface<FacetInterface>();
        }
    }
}
