using GraphQL.Types;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CurrencyType : ObjectGraphType<Currency>
    {
        public CurrencyType()
        {
            Field(x => x.Code, nullable: false).Description("Currency code may be used ISO 4217");
            Field(x => x.CultureName, nullable: false).Description("Culture name");
            Field(x => x.Symbol, nullable: false).Description("Symbol");
            Field(x => x.EnglishName, nullable: false).Description("English name");
            Field(x => x.ExchangeRate, nullable: false).Description("Exchange rate");
            Field(x => x.CustomFormatting, nullable: false).Description("Currency custom formatting");
        }
    }
}
