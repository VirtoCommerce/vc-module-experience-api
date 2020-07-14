using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class TaxDetailType : ObjectGraphType<TaxDetail>
    {
        public TaxDetailType()
        {
            Name = "TaxDetail";
            
            Field(d => d.Name);
            Field<DecimalGraphType> ("Amount" , resolve: context => context.Source.Amount);
            Field<DecimalGraphType> ("Rate", resolve: context => context.Source.Rate);
        }
    }
}
