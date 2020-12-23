using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class FacetTermType : ObjectGraphType<FacetTerm>
    {
        public FacetTermType()
        {
            Field(d => d.Term, nullable: true).Description("term");
            Field(d => d.Count, nullable: true).Description("count");
            Field(d => d.IsSelected, nullable: true).Description("is selected state");
            Field(d => d.Label);
        }
    }
}
