using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartShipmentItemType : ObjectGraphType<ShipmentItem>
    {
        public CartShipmentItemType()
        {
            Field(x => x.Quantity, nullable: true).Description("Quantity");
            Field(GraphTypeExtenstionHelper.GetActualType<LineItemType>(), "lineItem", resolve: context => context.Source.LineItem);
        }
    }
}
