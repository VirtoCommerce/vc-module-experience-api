using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Queries
{
    public class GetCartQuery: ObjectGraphType
    {
        public GetCartQuery(ICartBuilder cartBuilder)
        {
            FieldAsync<CartType>(
                "cart",
                "Get cart for current user",
                arguments: new QueryArguments(),
                resolve: async context =>
                {
                    await cartBuilder.LoadOrCreateNewTransientCartAsync(default, default, default, default, default);
                    await cartBuilder.ValidateAsync();
                    return cartBuilder.Cart;
                });
        }
    }
}
