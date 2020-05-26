using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class TermFacetResultType : ObjectGraphType<TermFacetResult>
    {
        public TermFacetResultType()
        {
            Name = "TermFacet";

            Field(d => d.Name, nullable: false).Description("The key/name  of the facet.");
            Field<FacetTypeEnum>("FacetType", "The three types of facets. Terms, Range, Filter");
            Field<ListGraphType<FacetTermType>>("Terms", "Terms", resolve: context => context.Source.Terms);

            IsTypeOf = obj => obj is TermFacetResult;
            Interface<FacetInterface>();
        }
    }
}
