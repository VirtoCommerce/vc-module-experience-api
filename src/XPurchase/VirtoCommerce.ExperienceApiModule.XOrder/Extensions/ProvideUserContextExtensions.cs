using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ProvideUserContextExtensions
    {
        public static CustomerOrderAggregate GetOrder(this IProvideUserContext userContext, string id = null)
        {
            return userContext.GetValue<CustomerOrderAggregate>(!string.IsNullOrEmpty(id) ? $"{nameof(CustomerOrderAggregate).ToCamelCase()}:{id}"
                : nameof(CustomerOrderAggregate).ToCamelCase());
        }

        public static Currency OrderCurrency<T>(this IResolveFieldContext<T> userContext)
        {
            if (userContext.Source is IEntity entity)
            {
                return userContext.GetValue<CustomerOrderAggregate>(entity.Id).Currency;
            }
            else if (userContext.Source is IValueObject valueObject)
            {
                return userContext.GetValue<CustomerOrderAggregate>(((ValueObject)valueObject).GetCacheKey()).Currency;
            }
            return null;
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
