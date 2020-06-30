using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputCartShipmentItemType : InputObjectGraphType<ShipmentItem>
    {
        public InputCartShipmentItemType()
        {
            Field(x => x.Quantity, nullable: true).Description("Quantity");
            Field<InputLineItemType>("lineItem", resolve: context => context.Source.LineItem);
        }
    }
}
