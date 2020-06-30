using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputMoneyType : InputObjectGraphType<Money>
    {
        public InputMoneyType()
        {
            Field(x => x.Amount, nullable: false).Description("A decimal with the amount rounded to the significant number of decimal digits.");
            Field(x => x.DecimalDigits, nullable: false).Description("Number of decimal digits for the associated currency.");
            Field(x => x.FormattedAmount, nullable: false).Description("Formatted amount.");
            Field(x => x.FormattedAmountWithoutCurrency, nullable: false).Description("Formatted amount without currency.");
            Field(x => x.FormattedAmountWithoutPoint, nullable: false).Description("Formatted amount without point.");
            Field(x => x.FormattedAmountWithoutPointAndCurrency, nullable: false).Description("Formatted amount without point and currency.");
        }
    }
}
