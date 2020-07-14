using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShipmentItemType : ObjectGraphType<ShipmentItem>
    {
        public OrderShipmentItemType()
        {
            Field(x => x.Id);
            Field(x => x.LineItemId);
            Field<LineItemType>(nameof(ShipmentItem.LineItem), resolve: context => context.Source.LineItem);
            Field(x => x.BarCode, true);
            Field(x => x.Quantity);
            Field(x => x.OuterId, true);
        }
    }
}
