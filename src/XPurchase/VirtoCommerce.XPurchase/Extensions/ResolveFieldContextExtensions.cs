using GraphQL;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Queries;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        public static CartAggregate GetCart(this IResolveFieldContext userContext)
        {
            return userContext.GetValueForSource<CartAggregate>();
        }

        public static Currency CartCurrency(this IResolveFieldContext userContext)
        {
            return userContext.GetValueForSource<CartAggregate>().Currency;
        }

        public static T GetCartQuery<T>(this IResolveFieldContext context) where T : ICartQuery
        {
            var result = AbstractTypeFactory<T>.TryCreateInstance();
            result.StoreId = context.GetArgumentOrValue<string>("storeId");
            result.UserId = context.GetArgumentOrValue<string>("userId");
            result.CurrencyCode = context.GetArgumentOrValue<string>("currencyCode");
            result.CultureName = context.GetArgumentOrValue<string>("cultureName");
            result.CartType = context.GetArgumentOrValue<string>("cartType");
            result.CartName = context.GetArgumentOrValue<string>("cartName");

            return result;
        }

        public static void SetSearchCartQuery(this IResolveFieldContext context, ICartQuery query)
        {
            context.UserContext["storeId"] = query.StoreId;
            context.UserContext["userId"] = query.UserId;
            context.UserContext["currencyCode"] = query.CurrencyCode;
            context.UserContext["cultureName"] = query.CultureName;
            context.UserContext["cartType"] = query.CartType;
            context.UserContext["cartName"] = query.CartName;
        }
    }
}
