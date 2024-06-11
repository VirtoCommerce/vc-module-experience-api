using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputRejectGiftItemsType : InputCartBaseType
    {
        public InputRejectGiftItemsType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("Ids",
                "Ids of gift lineItems to reject from cart");
        }
    }
}
