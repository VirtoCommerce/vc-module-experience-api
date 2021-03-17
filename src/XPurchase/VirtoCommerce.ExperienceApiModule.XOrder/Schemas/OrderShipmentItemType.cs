using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShipmentItemType : ObjectGraphType<ShipmentItem>
    {
        public OrderShipmentItemType()
        {
            Field(x => x.Id);
            Field(x => x.LineItemId);
            Field(GraphTypeExtenstionHelper.GetActualType<OrderLineItemType>(), nameof(ShipmentItem.LineItem), resolve: context => context.Source.LineItem);
            Field(x => x.BarCode, true);
            Field(x => x.Quantity);
            Field(x => x.OuterId, true);
        }
    }
}
