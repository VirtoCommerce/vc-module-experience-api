using GraphQL.Types;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputNewWishlistItemType : InputObjectGraphType<NewCartItem>
    {
        public InputNewWishlistItemType()
        {
            Field(x => x.ProductId, nullable: false).Description("Product Id");
            Field(x => x.Quantity, nullable: true).Description("Product quantity");
        }
    }
}
