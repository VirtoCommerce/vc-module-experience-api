using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using GraphQL;
using GraphQL.Execution;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        /// <summary>
        /// Get value from user context
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="resolveContext">GraphQL UserContext</param>
        /// <param name="key">Search key</param>
        /// <param name="defaultValue">Default return if value not founded in UserContext</param>
        /// <returns>Return value of type <typeparamref name="T"/> from UserContext or <paramref name="defaultValue"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T GetValue<T>(this IResolveFieldContext resolveContext, string key, T defaultValue)
        {
            if (resolveContext == null)
            {
                throw new ArgumentNullException(nameof(resolveContext));
            }

            if (resolveContext.UserContext.TryGetValue(key, out var value))
            {
                return castValue(value, defaultValue);
            }

            return defaultValue;

            static T castValue(object value, T defaultValue)
            {
                return value is ArgumentValue argumentValue ? (T)argumentValue.Value : castValueAsTyped(value, defaultValue);

                static T castValueAsTyped(object value, T defaultValue)
                {
                    return value is T typedObject ? typedObject : defaultValue;
                }
            }
        }

        public static T GetValue<T>(this IResolveFieldContext resolveContext, string key)
        {
            return resolveContext.GetValue(key, default(T));
        }

        public static bool IsAuthenticated(this IResolveFieldContext resolveContext)
        {
            return resolveContext.GetCurrentPrincipal().Identity?.IsAuthenticated == true;
        }

        public static string GetCurrentUserId(this IResolveFieldContext resolveContext)
        {
            var claimsPrincipal = GetCurrentPrincipal(resolveContext);
            return claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? claimsPrincipal?.FindFirstValue("name") ?? AnonymousUser.UserName;
        }

        public static ClaimsPrincipal GetCurrentPrincipal(this IResolveFieldContext resolveContext)
        {
            return ((GraphQLUserContext)resolveContext.UserContext).User;
        }

        public static T GetArgumentOrValue<T>(this IResolveFieldContext resolveContext, string key)
        {
            return resolveContext.GetArgument<T>(key) ?? resolveContext.GetValue<T>(key);
        }

        //PT-1606:  Need to check what there is no any alternative way to access to the original request arguments in sub selection
        public static void CopyArgumentsToUserContext(this IResolveFieldContext resolveContext)
        {
            if (resolveContext.Arguments.IsNullOrEmpty())
            {
                return;
            }

            foreach (var pair in resolveContext.Arguments)
            {
                resolveContext.UserContext.TryAdd(pair.Key, pair.Value);
            }
        }

        public static void SetExpandedObjectGraph<T>(this IResolveFieldContext resolveContext, T value)
        {
            var entities = value.GetFlatObjectsListWithInterface<IEntity>();

            foreach (var key in entities.Where(x => !string.IsNullOrEmpty(x.Id)))
            {
                resolveContext.UserContext.TryAdd(key.Id, value);
            }

            var valueObjects = value.GetFlatObjectsListWithInterface<IValueObject>();
            foreach (var valueObject in valueObjects)
            {
                resolveContext.UserContext.TryAdd(((ValueObject)valueObject).GetCacheKey(), value);
            }
        }

        public static TResult GetValueForSource<TResult>(this IResolveFieldContext resolveContext)
        {
            if (resolveContext == null)
            {
                throw new ArgumentNullException(nameof(resolveContext));
            }

            TResult result = default;

            if (resolveContext.Source is IEntity entity)
            {
                result = resolveContext.GetValue<TResult>(entity.Id);
            }
            else if (resolveContext.Source is IValueObject valueObject)
            {
                result = resolveContext.GetValue<TResult>(((ValueObject)valueObject).GetCacheKey());
            }

            return result;
        }

        public static void SetCurrencies(this IResolveFieldContext context, IEnumerable<Currency> currencies, string cultureName)
        {
            if (currencies == null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }

            var currenciesWithCulture = currencies.Select(x => currencies.GetCurrencyForLanguage(x.Code, cultureName)).ToArray();

            context.UserContext["allCurrencies"] = currenciesWithCulture;
        }

        public static void SetCurrency(this IResolveFieldContext context, Currency currency)
        {
            if (currency == null)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            context.UserContext["currencyCode"] = currency.Code;
        }

        public static Currency GetCurrencyByCode<T>(this IResolveFieldContext<T> userContext, string currencyCode)
        {
            var allCurrencies = userContext.GetValue<IEnumerable<Currency>>("allCurrencies");
            var result = allCurrencies?.FirstOrDefault(x => x.Code.EqualsInvariant(currencyCode));
            if (result == null)
            {
                throw new OperationCanceledException($"the currency with code '{currencyCode}' is not registered");
            }

            return result;
        }

        public static T GetDynamicPropertiesQuery<T>(this IResolveFieldContext context) where T : IDynamicPropertiesQuery
        {
            var result = AbstractTypeFactory<T>.TryCreateInstance();
            result.CultureName = context.GetCultureName();
            return result;
        }

        public static string GetCultureName(this IResolveFieldContext context)
        {
            return context.GetArgumentOrValue<string>(Constants.CultureName);
        }
    }
}
