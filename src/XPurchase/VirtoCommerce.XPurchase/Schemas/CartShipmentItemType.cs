using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartShipmentItemType : ExtendableAuditableEntityType<ShipmentItem>
    {
        public CartShipmentItemType()
        {
            Field(x => x.Quantity, nullable: false).Description("Quantity");
            ExtendableField<LineItemType>("lineItem", resolve: context => context.Source.LineItem);
        }
    }
}
