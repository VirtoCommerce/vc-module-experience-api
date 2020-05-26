using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class FacetTermType : ObjectGraphType<FacetTerm>
    {
        public FacetTermType()
        {
            Field(d => d.Term, nullable: true).Description("term");
            Field(d => d.Count, nullable: true).Description("count");
            Field(d => d.IsSelected, nullable: true).Description("is selected state");           
        }

    }
}
