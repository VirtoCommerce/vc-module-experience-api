using GraphQL;
using GraphQL.Types;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class GraphQLContextExtensions
    {
        public static T GetCartCommand<T>(this IResolveFieldContext<CartAggregate> context)
            where T : CartCommand => context.GetArgument<T>(PurchaseSchema._commandName);
    }
}
