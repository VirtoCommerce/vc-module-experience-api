using GraphQL.Execution;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        public static CartAggregate GetCart(this IResolveFieldContext userContext)
        {
            return userContext.GetValue<CartAggregate>("cartAggregate");
        }

        public static Currency CartCurency(this IResolveFieldContext userContext)
        {
            return userContext.GetValue<CartAggregate>("cartAggregate").Currency;
        }
    }
}
