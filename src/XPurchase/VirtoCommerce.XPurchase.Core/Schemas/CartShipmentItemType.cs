using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class CartShipmentItemType : ExtendableGraphType<ShipmentItem>
    {
        public CartShipmentItemType()
        {
            Field(x => x.Quantity, nullable: false).Description("Quantity");
            ExtendableField<LineItemType>("lineItem", resolve: context => context.Source.LineItem);
        }
    }
}
