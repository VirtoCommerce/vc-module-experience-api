using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Factories;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models;
using VirtoCommerce.ExperienceApiModule.XPurchase.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Queries
{
    public class GetCartQuery: ObjectGraphType
    {
        public GetCartQuery(IShoppingCartAggregateFactory cartFactory)
        {
            FieldAsync<CartType>(
                "cart",
                "Get cart for current user",
                arguments: new QueryArguments(),
                resolve: async context =>
                {
                    var cartAggregate = await cartFactory.CreateOrGetShoppingCartAggregateAsync(new ShoppingCartContext());
                    await cartAggregate.ValidateAsync();
                    return cartAggregate.Cart;
                });
        }
    }
}
