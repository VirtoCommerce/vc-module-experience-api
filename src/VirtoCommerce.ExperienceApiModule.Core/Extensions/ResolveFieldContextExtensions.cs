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
        public static string GetCultureName<T>(this IResolveFieldContext<T> context, string defaultValue = default)
        {
            var cultureName = context.GetArgument<string>(Constants.CultureName);
            if (cultureName != null)
            {
                return cultureName;
            }

            var cultureNameFromContext = context.GetValue<string>(Constants.CultureName, null);
            if (cultureNameFromContext != null)
            {
                return cultureNameFromContext;
            }

            return defaultValue;
        }

        public static Language GetLanguage<T>(this IResolveFieldContext<T> context, Language defaultValue = default)
        {
            var cultureName = context.GetCultureName();
            if (cultureName != null)
            {
                var language = new Language(cultureName);
                return language;
            }

            var languageFromContext = context.GetValue<Language>("language", null);
            if (languageFromContext != null)
            {
                return languageFromContext;
            }

            return defaultValue;
        }

        /// <summary>
        /// Get saved currency from arguments or user context
        /// </summary>
        public static Currency GetCurrency<T>(this IResolveFieldContext<T> context, Currency defaultValue = default)
        {
            var currencyCode = context.GetArgument<string>(Constants.CurrencyCode);
            if (currencyCode != null)
            {
                return new Currency(context.GetLanguage(), currencyCode);
            }

            var currencyFromContext = context.GetValue<Currency>("currency", null);
            if (currencyFromContext != null)
            {
                return currencyFromContext;
            }

            return defaultValue;
        }
    }
}
