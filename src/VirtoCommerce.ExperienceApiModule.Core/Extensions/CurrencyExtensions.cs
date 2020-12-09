using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class CurrencyExtensions
    {
        public static Currency GetCurrencyForLanguage(this IEnumerable<Currency> currencies, string currencyCode, string language)
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
            var defaultLanguage = !string.IsNullOrEmpty(language) ? new Language(language) : Language.InvariantLanguage;
            //Clone  currency with cart language
            currency = new Currency(language != null ? new Language(language) : defaultLanguage, currency.Code, currency.Name, currency.Symbol, currency.ExchangeRate)
            {
                CustomFormatting = currency.CustomFormatting
            };
            return currency;
        }
    }
}
