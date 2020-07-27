using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphQL;
using GraphQL.Execution;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class ProvideUserContextExtensions
    {
        /// <summary>
        /// Get value from user context
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="userContext">GraphQL UserContext</param>
        /// <param name="key">Search key</param>
        /// <param name="defaultValue">Default return if value not founded in UserContext</param>
        /// <returns>Return value of type <typeparamref name="T"/> from UserContext or <paramref name="defaultValue"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T GetValue<T>(this IProvideUserContext userContext, string key, T defaultValue)
        {
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));

            return userContext.UserContext.TryGetValue(key, out var value)
                ? (T)value
                : defaultValue;
        }

        /// <summary>
        /// Get value from user context
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="userContext">GraphQL UserContext</param>
        /// <param name="key">Search key</param>
        /// <returns>Return value of type <typeparamref name="T"/> from UserContext</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public static T GetValue<T>(this IProvideUserContext userContext, string key)
        {
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));

            return userContext.UserContext.TryGetValue(key, out var value)
                ? (T)value
                : throw new KeyNotFoundException(key);
        }

        public static void SetValue(this IProvideUserContext userContext, object objectForSave, string keyName = null)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException(nameof(userContext));
            }

            var key = keyName ?? objectForSave.GetType().Name;

            if (!userContext.UserContext.TryGetValue(key, out _))
            {
                userContext.UserContext.Add(KeyValuePair.Create(key, objectForSave));
            }
        }

        public static void SetValue<T>(this IProvideUserContext userContext, T value)
        {
            var entities = value.GetFlatObjectsListWithInterface<IEntity>();

            foreach (var key in entities.Where(x => !string.IsNullOrEmpty(x.Id)))
            {
                userContext.UserContext.TryAdd(key.Id, value);
            }

            var valueObjects = value.GetFlatObjectsListWithInterface<IValueObject>();
            foreach (var valueObject in valueObjects)
            {
                userContext.UserContext.TryAdd(((ValueObject)valueObject).GetCacheKey(), value);
            }
        }
    }
}
