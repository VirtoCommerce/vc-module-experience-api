using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class CurrencyExtensions
    {
        public static Currency GetCurrencyForLanguage(this IEnumerable<Currency> currencies, string currencyCode, string cultureName)
        {
            if (currencies == null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }

            var currency = currencies.FirstOrDefault(x => x.Code.EqualsInvariant(currencyCode));
            if (currency == null)
            {
                throw new OperationCanceledException($"currency with code: {currencyCode} is not registered in the system");
            }

            //Clone currency with specified language
            return new Currency(
                cultureName != null ? new Language(cultureName) : Language.InvariantLanguage,
                currency.Code,
                currency.Name,
                currency.Symbol,
                currency.ExchangeRate
            )
            {
                CustomFormatting = currency.CustomFormatting,
                RoundingPolicy = currency.RoundingPolicy,
                RoundingType = currency.RoundingType,
                MidpointRounding = currency.MidpointRounding
            };
        }
    }
}
