using GraphQL.Types;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartShipmentItemType : ObjectGraphType<CartShipmentItem>
    {
        public CartShipmentItemType()
        {
            Field(x => x.Quantity, nullable: true).Description("Quantity");
            Field<LineItemType>("lineItem", resolve: context => context.Source.LineItem);
        }
    }
}
