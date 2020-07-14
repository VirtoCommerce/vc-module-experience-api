using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderTaxDetailType : ObjectGraphType<TaxDetail>
    {
        public OrderTaxDetailType()
        {
            Field(x => x.Rate);
            Field(x => x.Amount);
            Field(x => x.Name);
        }
    }
}
