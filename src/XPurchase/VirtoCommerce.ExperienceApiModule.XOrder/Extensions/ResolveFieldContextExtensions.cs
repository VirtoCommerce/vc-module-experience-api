using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        public static Currency GetOrderCurrency<T>(this IResolveFieldContext<T> userContext)
        {
           return userContext.GetValueForSource<CustomerOrderAggregate>().Currency;
        }

    }
}
