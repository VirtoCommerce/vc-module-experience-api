using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputOrderShipmentItemType : InputObjectGraphType<ShipmentItem>
    {
        public InputOrderShipmentItemType()
        {
            Field(x => x.Id);
            Field(x => x.LineItemId);
            Field<InputOrderLineItemType>(nameof(ShipmentItem.LineItem));
            Field(x => x.BarCode, true);
            Field(x => x.Quantity);
            Field(x => x.OuterId, true);
        }
    }
}
