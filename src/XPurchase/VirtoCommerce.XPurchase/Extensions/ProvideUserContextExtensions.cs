using GraphQL.Execution;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

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
    }
}
