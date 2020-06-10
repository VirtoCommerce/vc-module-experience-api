using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class CartType : ObjectGraphType<ShoppingCart>
    {
        public CartType()
        {
            Field(x => x.Name, nullable: false).Description("Shopping cart name"); // todo add more fields
        }
    }
}
