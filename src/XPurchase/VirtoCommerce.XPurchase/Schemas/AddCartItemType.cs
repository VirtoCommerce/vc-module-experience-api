using GraphQL.Types;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class AddCartItemType : InputObjectGraphType<AddCartItem>
    {
        public AddCartItemType()
        {
            Field(x => x.ProductId, nullable: false).Description("Product id");
            Field(x => x.Quantity, nullable: false).Description("Product quantity");
            Field(x => x.Price, nullable: true).Description("Product manual price");
            Field(x => x.Comment, nullable: true).Description("Product comment");
            //Field<ListGraphType<DynamicPropertyType>>("dynamicProperties", resolve: context => context.Source.DynamicProperties); //todo add dynamic properties
        }
    }
}
