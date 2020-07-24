using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderCurrencyType : ObjectGraphType<Currency>
    {
        public OrderCurrencyType()
        {
            //Field(x => x.Name);
            //Field(x => x.IsPrimary);
            Field(x => x.Code);
            Field(x => x.Symbol, true);
            Field(x => x.ExchangeRate, true);
            Field(x => x.CustomFormatting, true);
        }
    }
}
