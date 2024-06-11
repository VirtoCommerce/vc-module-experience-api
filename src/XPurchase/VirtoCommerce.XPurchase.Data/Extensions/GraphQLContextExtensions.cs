using GraphQL;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Data.Schemas;

namespace VirtoCommerce.XPurchase.Data.Extensions
{
    public static class GraphQLContextExtensions
    {
        public static T GetCartCommand<T>(this IResolveFieldContext<CartAggregate> context)
            where T : CartCommand => (T)context.GetArgument(GenericTypeHelper.GetActualType<T>(), PurchaseSchema.CommandName);
    }
}
