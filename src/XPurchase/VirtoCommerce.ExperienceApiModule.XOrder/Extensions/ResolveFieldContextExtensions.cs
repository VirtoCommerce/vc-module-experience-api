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
            var allCurrencies = userContext.GetValue<IEnumerable<Currency>>("allCurrencies");
            if (allCurrencies == null)
            {
                throw new OperationCanceledException("allCurrencies is not found in the userContext");
            }
            Currency result = null;
            if (string.IsNullOrEmpty(currencyCode))
            {
                var order = userContext.GetValueForSource<CustomerOrderAggregate>();
                if (order == null)
                {
                    throw new OperationCanceledException("unable to get currency from order, currency can't be null");
                }
            }
            result = allCurrencies.FirstOrDefault(x => x.Code.EqualsInvariant(currencyCode));
            if (result == null)
            {
                throw new OperationCanceledException($"the currency with code { currencyCode } is not registered");
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
