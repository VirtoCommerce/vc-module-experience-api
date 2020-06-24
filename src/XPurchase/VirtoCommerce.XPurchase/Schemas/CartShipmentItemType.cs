using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartShipmentItemType : ObjectGraphType<ShipmentItem>
    {
        public CartShipmentItemType()
        {
            Field(x => x.Quantity, nullable: true).Description("Quantity");
            Field<LineItemType>("lineItem", resolve: context => context.Source.LineItem);
        }
    }
}
