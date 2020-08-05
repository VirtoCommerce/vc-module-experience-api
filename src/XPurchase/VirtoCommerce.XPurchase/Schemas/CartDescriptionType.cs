using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartDescriptionType : ObjectGraphType<CartDescription>
    {
        public CartDescriptionType()
        {
            Field(x => x.Id, nullable: true).Description("Cart description list Id");
            Field(x => x.Name, nullable: false).Description("Cart description list name");
            Field(x => x.Status, nullable: true).Description("Cart description list status");
            Field(x => x.StoreId, nullable: true).Description("Cart description list store id");
            Field(x => x.CustomerId, nullable: true).Description("Cart description list user id");
            Field(x => x.Currency, nullable: true).Description("Cart description list currency");
            Field(x => x.LanguageCode, nullable: true).Description("Cart description list language");
            Field(x => x.Type, nullable: true).Description("Cart description list type");
        }
    }
}
