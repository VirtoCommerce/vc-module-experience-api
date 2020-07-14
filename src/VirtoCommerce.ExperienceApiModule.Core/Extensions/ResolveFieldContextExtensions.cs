using System;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        public static Language GetLanguage<T>(this IResolveFieldContext<T> context)
        {
            var cultureName = context.GetArgument<string>(Constants.CultureName);
            if (cultureName != null)
            {
                var language = new Language(cultureName);
                context.SaveValue(language);
                return language;
            }

            var languageFromContext = context.GetValue<Language>("language");
            if (languageFromContext != null)
            {
                return languageFromContext;
            }

            throw new ArgumentException("Language not found in arguments or context");
        }

        /// <summary>
        /// Get saved currency from arguments or user context
        /// </summary>
        public static Currency GetCurrency<T>(this IResolveFieldContext<T> context)
        {
            var currencyCode = context.GetArgument<string>(Constants.CurrencyCode);
            if (currencyCode != null)
            {
                var currency = new Currency(context.GetLanguage(), currencyCode);
                context.SaveValue(currency);
                return currency;
            }

            var currencyFromContext = context.GetValue<Currency>("currency");
            if (currencyFromContext != null)
            {
                return currencyFromContext;
            }

            throw new ArgumentException("Currency not found in arguments or context");
        }
    }
}
