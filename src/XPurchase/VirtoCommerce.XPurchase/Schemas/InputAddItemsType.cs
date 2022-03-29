using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddItemsType : InputCartBaseType
    {
        public InputAddItemsType()
        {
            Field<NonNullGraphType<ListGraphType<InputNewCartItemType>>>("CartItems",
                "Cart items");
        }
    }
}
