using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShipmentItemType : ExtendableGraphType<ShipmentItem>
    {
        public OrderShipmentItemType()
        {
            Field(x => x.Id);
            Field(x => x.LineItemId);
            ExtendableField<OrderLineItemType>(nameof(ShipmentItem.LineItem), resolve: context => context.Source.LineItem);
            Field(x => x.BarCode, true);
            Field(x => x.Quantity);
            Field(x => x.OuterId, true);
            Field(x => x.Status, true);
        }
    }
}
