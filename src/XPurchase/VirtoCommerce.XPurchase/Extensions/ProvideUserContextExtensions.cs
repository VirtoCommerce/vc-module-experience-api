using System;
using System.Collections.Generic;
using GraphQL.Execution;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ProvideUserContextExtensions
    {
        public static CartAggregate GetCart(this IProvideUserContext userContext)
        {
            return userContext.GetValue<CartAggregate>("cartAggregate");
        }

        public static Currency CartCurency(this IProvideUserContext userContext)
        {
            return userContext.GetValue<CartAggregate>("cartAggregate").Currency;
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
