using GraphQL.Types;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputNewBulkItemType : InputObjectGraphType<NewBulkCartItem>
    {
        public InputNewBulkItemType()
        {
            Field(x => x.ProductSku, nullable: false).Description("Product SKU");
            Field(x => x.Quantity, nullable: true).Description("Product quantity");
        }
    }
}
