using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRejectGiftItemsType : InputCartBaseType
    {
        public InputRejectGiftItemsType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("GiftItemIds");
        }
    }
}
