using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        public static Currency GetOrderCurrency<T>(this IResolveFieldContext<T> userContext)
        {
            return userContext.GetValueForSource<CustomerOrderAggregate>().Currency;
        }

        public static Currency GetCurrencyByCode<T>(this IResolveFieldContext<T> userContext, string currencyCode)
        {
            //Try to get a currency from order if currency code is not set explicitly or undefined
            var result = userContext.GetOrderCurrency();
            //If the passed currency differs from the order currency, we try to find it from the all registered currencies.
            if (result == null || (!string.IsNullOrEmpty(currencyCode) && !result.Code.EqualsInvariant(currencyCode)))
            {
                var allCurrencies = userContext.GetValue<IEnumerable<Currency>>("allCurrencies");
                result = allCurrencies?.FirstOrDefault(x => x.Code.EqualsInvariant(currencyCode));
            }
            if (result == null)
            {
                throw new OperationCanceledException($"the currency with code '{ currencyCode }' is not registered");
            }
            return result;
        }

        public static void SetCurrencies(this IResolveFieldContext context, IEnumerable<Currency> currencies, string cultureName)
        {
            if (currencies == null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }
            var currenciesWithCulture = currencies.Select(x => new Currency(cultureName != null ? new Language(cultureName) : Language.InvariantLanguage, x.Code, x.Name, x.Symbol, x.ExchangeRate)
            {
                CustomFormatting = x.CustomFormatting
            }).ToArray();
            context.UserContext["allCurrencies"] = currenciesWithCulture;
        }
    }
}
