using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class MoneyExtensions
    {
        public static Money ToMoney(this decimal amount, Currency currency)
        {
            return new Money(amount, currency);
        }
    }
}
