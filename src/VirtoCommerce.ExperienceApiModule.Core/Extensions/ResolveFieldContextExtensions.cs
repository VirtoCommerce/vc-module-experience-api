using System;
using System.Text;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Common;

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

        /// <summary>
        /// Get value of type <typeparamref name="U"/> from UserContext by building key from <typeparamref name="T"/> type
        /// </summary>
        /// <typeparam name="T">User context type</typeparam>
        /// <typeparam name="U">Returned type</typeparam>
        /// <param name="resolveFieldContext">GraphQL filed context (contains user context)</param>
        /// <returns>Value from user context of <typeparamref name="U"/> type</returns>
        public static U GetValue<T, U>(this IResolveFieldContext<T> resolveFieldContext)
        {
            if (resolveFieldContext == null) throw new ArgumentNullException(nameof(resolveFieldContext));

            var keyBuilder = new StringBuilder();

            if (resolveFieldContext.Source is IEntity entity)
            {
                keyBuilder.Append(typeof(T).Name.ToCamelCase());

                if (!string.IsNullOrEmpty(entity.Id))
                {
                    keyBuilder.Append($":{entity.Id}");
                }
            }
            else if (resolveFieldContext.Source is IValueObject valueObject)
            {
                keyBuilder.Append(((ValueObject)valueObject).GetCacheKey());
            }

            return resolveFieldContext.GetValue<U>(keyBuilder.ToString());
        }
    }
}
