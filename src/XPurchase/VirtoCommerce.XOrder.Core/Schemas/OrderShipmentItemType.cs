using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class OrderShipmentItemType : ExtendableGraphType<ShipmentItem>
    {
        public OrderShipmentItemType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.LineItemId, nullable: true);
            ExtendableField<OrderLineItemType>(nameof(ShipmentItem.LineItem), resolve: context => context.Source.LineItem);
            Field(x => x.BarCode, nullable: true);
            Field(x => x.Quantity, nullable: false);
            Field(x => x.OuterId, nullable: true);
            Field(x => x.Status, nullable: true);
        }
    }
}
