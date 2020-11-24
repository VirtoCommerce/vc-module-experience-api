using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
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
                return value is T ? (T)value : defaultValue;
            }
            return defaultValue;
        }

        public static T GetValue<T>(this IResolveFieldContext resolveContext, string key)
        {
            return resolveContext.GetValue(key, default(T));
        }

        public static string GetCurrentUserId(this IResolveFieldContext resolveContext)
        {
            var claimsPrincipal = ((GraphQLUserContext)resolveContext.UserContext).User;
            return claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? claimsPrincipal?.FindFirstValue("name");
        }

        public static ClaimsPrincipal GetCurrentPrincipal(this IResolveFieldContext resolveContext)
        {
            return ((GraphQLUserContext)resolveContext.UserContext).User;
        }

        public static T GetArgumentOrValue<T>(this IResolveFieldContext resolveContext, string key)
        {
            return resolveContext.GetArgument<T>(key) ?? resolveContext.GetValue<T>(key);
        }

        //TODO:  Need to check what there is no any alternative way to access to the original request arguments in sub selection
        public static void CopyArgumentsToUserContext(this IResolveFieldContext resolveContext)
        {
            foreach(var pair in resolveContext.Arguments)
            {
                resolveContext.UserContext[pair.Key] = pair.Value;
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


    }
}
