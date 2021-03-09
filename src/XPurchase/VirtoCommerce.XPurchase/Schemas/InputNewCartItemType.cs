using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputNewCartItemType : InputObjectGraphType<NewCartItem>
    {
        public InputNewCartItemType()
        {
            Field(x => x.ProductId, false).Description("Product Id");
            Field(x => x.Quantity, true).Description("Product quantity");
        }
    }
}
