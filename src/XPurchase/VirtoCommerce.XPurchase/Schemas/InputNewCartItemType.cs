using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputNewCartItemType : InputObjectGraphType<NewCartItem>
    {
        public InputNewCartItemType()
        {
            Field(x => x.ProductId, nullable: false).Description("Product Id");
            Field(x => x.Quantity, nullable: true).Description("Product quantity");
        }
    }
}
