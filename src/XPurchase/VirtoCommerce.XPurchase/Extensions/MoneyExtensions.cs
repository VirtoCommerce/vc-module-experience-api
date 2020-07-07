using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.XPurchase.Extensions
{
    //TODO: Remove later, only as temporary workaround, need to costruct money from proper store currency and given language
    public static class MoneyExtensions
    {
        public static Money ToMoney(this decimal amount, Currency currency)
        {
            return new Money(amount, currency);
        }
    }
}
