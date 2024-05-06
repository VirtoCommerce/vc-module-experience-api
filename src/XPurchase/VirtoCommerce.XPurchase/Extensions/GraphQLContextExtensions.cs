using GraphQL;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class GraphQLContextExtensions
    {
        public static T GetCartCommand<T>(this IResolveFieldContext<CartAggregate> context)
            where T : CartCommand => (T)context.GetArgument(GenericTypeHelper.GetActualType<T>(), PurchaseSchema._commandName);
    }
}
