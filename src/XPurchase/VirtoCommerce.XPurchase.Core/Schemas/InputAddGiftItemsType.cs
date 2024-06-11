using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddGiftItemsType : InputCartBaseType
    {
        public InputAddGiftItemsType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("Ids",
                "IDs of gift rewards to add to the cart");
        }
    }
}
