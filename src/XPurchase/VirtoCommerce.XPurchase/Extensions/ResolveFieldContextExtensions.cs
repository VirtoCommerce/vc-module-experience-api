using System;
using GraphQL;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Queries;
using VirtoCommerce.XPurchase.Schemas;

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

        public static Money GetTotal(this IResolveFieldContext<CartAggregate> context, decimal number)
        {
            return context.Source.HasSelectedLineItems
                ? number.ToMoney(context.Source.Currency)
                : new Money(0.0m, context.Source.Currency);
        }

        public static T GetCartCommand<T>(this IResolveFieldContext context)
            where T : ICartRequest
        {
            var cartCommand = (T)context.GetArgument(GenericTypeHelper.GetActualType<T>(), PurchaseSchema._commandName);

            if (cartCommand != null)
            {
                cartCommand.OrganizationId = context.GetCurrentOrganizationId();
            }

            return cartCommand;
        }

        public static T GetCartQuery<T>(this IResolveFieldContext context)
            where T : ICartRequest
        {
            var result = AbstractTypeFactory<T>.TryCreateInstance();
            result.StoreId = context.GetArgumentOrValue<string>("storeId");
            result.UserId = context.GetArgumentOrValue<string>("userId");
            result.OrganizationId = context.GetCurrentOrganizationId();
            result.CurrencyCode = context.GetArgumentOrValue<string>("currencyCode");
            result.CultureName = context.GetArgumentOrValue<string>("cultureName");
            result.CartType = context.GetArgumentOrValue<string>("cartType");
            result.CartName = context.GetArgumentOrValue<string>("cartName");

            return result;
        }

        [Obsolete("Not being called", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
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
