using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class WishListType : ObjectGraphType<WishList>
    {
        public WishListType()
        {
            Field(x => x.Id, nullable: true).Description("Wish list Id");
            Field(x => x.Name, nullable: false).Description("Wish list name");
            Field(x => x.Status, nullable: true).Description("Wish list status");
            Field(x => x.StoreId, nullable: true).Description("Wish list store id");
            Field(x => x.CustomerId, nullable: true).Description("Wish list user id");
            Field(x => x.Currency, nullable: true).Description("Wish list currency");
            Field(x => x.LanguageCode, nullable: true).Description("Wish list language");
            Field(x => x.Type, nullable: true).Description("Wish list type");
        }
    }
}
