using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class TaxDetailType : ObjectGraphType<TaxDetail>
    {
        public TaxDetailType()
        {
            Name = "Tax Detail";
            
            Field(d => d.Name);
            Field(d => d.Amount);
            Field(d => d.Rate);



        }
    }
}
