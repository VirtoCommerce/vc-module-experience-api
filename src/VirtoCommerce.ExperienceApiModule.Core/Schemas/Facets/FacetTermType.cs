using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.Facets
{
    public class FacetTermType : ObjectGraphType<FacetTerm>
    {
        public FacetTermType()
        {
            Field(d => d.Term, nullable: false).Description("term");
            Field(d => d.Count, nullable: false).Description("count");
            Field(d => d.IsSelected, nullable: false).Description("is selected state");
            Field(d => d.Label, nullable: false);
        }
    }
}
