using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ProvideUserContextExtensions
    {
        private static string _keyPart = $".{nameof(IEntity.Id)}";

        public static CustomerOrderAggregate GetOrder(this IProvideUserContext userContext, string id = null)
        {
            return userContext.GetValue<CustomerOrderAggregate>(!string.IsNullOrEmpty(id) ? $"{nameof(CustomerOrderAggregate).ToCamelCase()}:{id}"
                : nameof(CustomerOrderAggregate).ToCamelCase());
        }

        public static Currency OrderCurrency<T>(this IResolveFieldContext<T> userContext)
        {
            return userContext.GetValue<CustomerOrderAggregate>(((IEntity)userContext.Source).Id).Currency;
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
            var values = value.TraverseObjectGraph();

            foreach (var keyValue in values.Where(x => x.Key.Contains($".{nameof(IEntity.Id)}") && x.Value != null))
            {
                userContext.UserContext.TryAdd(keyValue.Value.ToString(), value);
            }
        }
    }
}
