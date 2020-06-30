using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputCurrencyType : InputObjectGraphType<Currency>
    {
        public InputCurrencyType()
        {
            //TODO:
            //Field(x => x.CultureName, nullable: false).Description("Culture name");
            //Field(x => x.EnglishName, nullable: false).Description("English name");

            Field(x => x.Code, nullable: false).Description("Currency code may be used ISO 4217");
            Field(x => x.CustomFormatting, nullable: false).Description("Currency custom formatting");
            Field(x => x.ExchangeRate, nullable: false).Description("Exchange rate");
            Field(x => x.Symbol, nullable: false).Description("Symbol");
        }
    }
}
