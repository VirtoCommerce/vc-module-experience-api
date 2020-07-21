using System;
using System.Collections.Generic;
using GraphQL.Execution;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class ProvideUserContextExtensions
    {
        public static T GetValue<T>(this IProvideUserContext userContext, string key, T defaultValue = default)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException(nameof(userContext));
            }

            return userContext.UserContext.TryGetValue(key, out var value) ? (T)value : defaultValue;
        }

        public static void SaveValue(this IProvideUserContext userContext, object objectForSave, string keyName = null)
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
    }
}
