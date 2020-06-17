using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class CartShipmentItemType : ObjectGraphType<CartShipmentItem>
    {
        public CartShipmentItemType()
        {
            Field(x => x.Quantity, nullable: true).Description("Quantity");
            Field<ObjectGraphType<LineItemType>>("lineItem", resolve: context => context.Source.LineItem);
        }
    }
}
