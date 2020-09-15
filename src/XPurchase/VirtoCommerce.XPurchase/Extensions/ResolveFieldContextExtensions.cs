using GraphQL;
using GraphQL.Types;
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
            return userContext.GetValue<CartAggregate>("cartAggregate");
        }

        public static Currency CartCurrency(this IResolveFieldContext userContext)
        {
            return userContext.GetValue<CartAggregate>("cartAggregate").Currency;
        }

        public static T GetSearchCartQuery<T>(this IResolveFieldContext context) where T : ICartQuery
        {
            var result = AbstractTypeFactory<T>.TryCreateInstance();
            result.StoreId = context.GetArgument<string>("storeId") ?? context.GetValue<string>("storeId");
            result.UserId = context.GetArgument<string>("userId") ?? context.GetValue<string>("userId");
            result.CurrencyCode = context.GetArgument<string>("currencyCode") ?? context.GetValue<string>("currencyCode");
            result.CultureName = context.GetArgument<string>("cultureName") ?? context.GetValue<string>("cultureName");
            result.CartType = context.GetArgument<string>("cartType") ?? context.GetValue<string>("cartType");
            result.CartName = context.GetArgument<string>("cartName") ?? context.GetValue<string>("cartName");

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
