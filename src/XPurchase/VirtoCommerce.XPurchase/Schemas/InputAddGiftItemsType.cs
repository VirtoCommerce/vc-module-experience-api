using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddGiftItemsType : InputCartBaseType
    {
        public InputAddGiftItemsType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("Ids", "Ids of gift rewards to add to cart");
        }
    }
}
