using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Execution;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ProvideUserContextExtensions
    {
        public static CustomerOrderAggregate GetOrder(this IProvideUserContext userContext, string id = null)
        {
            return userContext.GetValue<CustomerOrderAggregate>(!string.IsNullOrEmpty(id) ? $"{nameof(CustomerOrderAggregate).ToCamelCase()}:{id}"
                : nameof(CustomerOrderAggregate).ToCamelCase());
        }

        public static Currency OrderCurency(this IProvideUserContext userContext, string id = null)
        {
            return userContext.GetValue<CustomerOrderAggregate>(!string.IsNullOrEmpty(id) ? $"{nameof(CustomerOrderAggregate).ToCamelCase()}:{id}"
                : nameof(CustomerOrderAggregate).ToCamelCase()).Currency;
        }

        public static T GetValue<T>(this IProvideUserContext userContext, string key)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException(nameof(userContext));
            }
            if (userContext.UserContext.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            throw new KeyNotFoundException(key);
        }
    }
}
