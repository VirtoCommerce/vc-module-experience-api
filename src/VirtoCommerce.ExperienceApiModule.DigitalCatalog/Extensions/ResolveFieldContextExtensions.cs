using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        public static T GetCatalogQuery<T>(this IResolveFieldContext context) where T : ICatalogQuery
        {
            var result = AbstractTypeFactory<T>.TryCreateInstance();
            result.StoreId = context.GetArgument<string>("storeId") ?? context.GetValue<string>("storeId");
            result.UserId = context.GetArgument<string>("userId") ?? context.GetValue<string>("userId") ?? context.GetCurrentUserId();
            result.CurrencyCode = context.GetArgument<string>("currencyCode") ?? context.GetValue<string>("currencyCode");
            result.CultureName = context.GetArgument<string>("cultureName") ?? context.GetValue<string>("cultureName");

            return result;
        }

        public static void SetCatalogQuery(this IResolveFieldContext context, ICatalogQuery query)
        {
            context.UserContext["storeId"] = query.StoreId;
            context.UserContext["userId"] = query.UserId;
            context.UserContext["currencyCode"] = query.CurrencyCode;
            context.UserContext["cultureName"] = query.CultureName;
        }
    }
}
