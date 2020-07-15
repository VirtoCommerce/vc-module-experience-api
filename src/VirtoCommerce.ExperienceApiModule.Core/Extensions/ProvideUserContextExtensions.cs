using System;
using System.Collections.Generic;
using GraphQL.Execution;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class ProvideUserContextExtensions
    {
        public static T GetValue<T>(this IProvideUserContext userContext, string key, bool nullable = false)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException(nameof(userContext));
            }

            if (userContext.UserContext.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            if (nullable)
            {
                return default;
            }

            throw new KeyNotFoundException(key);
        }

        public static void SaveValue(this IProvideUserContext userContext, object objectForSave)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException(nameof(userContext));
            }

            var key = objectForSave.GetType().Name;

            if (!userContext.UserContext.TryGetValue(key, out _))
            {
                userContext.UserContext.Add(KeyValuePair.Create(key, objectForSave));
            }
        }
    }
}
