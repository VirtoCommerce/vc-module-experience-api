using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class GiftItemType : ExtendableGraphType<CartGiftItem>
    {
        public GiftItemType()
        {
            Field(x => x.ImageUrl, nullable: true).Description("Product image absolute URL");
            Field(x => x.ProductId).Description("Product id");
            Field(x => x.Name).Description("Associated product name");
            Field(x => x.IsAccepted).Description("Flag whether this gift was added into cart");
        }
    }
}
