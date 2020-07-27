using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ProvideUserContextExtensions
    {
        public static Currency OrderCurrency<T>(this IResolveFieldContext<T> userContext)
            => userContext.GetValue<T, CustomerOrderAggregate>().Currency;
    }
}
