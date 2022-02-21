using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
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
