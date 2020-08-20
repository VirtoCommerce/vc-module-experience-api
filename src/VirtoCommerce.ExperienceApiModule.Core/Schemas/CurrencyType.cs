using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class CurrencyType : ObjectGraphType<Currency>
    {
        public CurrencyType()
        {
            Field(x => x.Code, nullable: false).Description("Currency code may be used ISO 4217");
            Field(x => x.CultureName, nullable: true).Description("Culture name");
            Field(x => x.Symbol, nullable: true).Description("Symbol");
            Field(x => x.EnglishName, nullable: true).Description("English name");
            Field(x => x.ExchangeRate, nullable: true).Description("Exchange rate");
            Field(x => x.CustomFormatting, nullable: true).Description("Currency custom formatting");
        }
    }
}
