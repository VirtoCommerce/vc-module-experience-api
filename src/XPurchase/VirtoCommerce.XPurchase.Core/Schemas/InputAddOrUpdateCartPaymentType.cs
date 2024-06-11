using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddOrUpdateCartPaymentType : InputCartBaseType
    {
        public InputAddOrUpdateCartPaymentType()
        {
            Field<NonNullGraphType<InputPaymentType>>("payment",
                "Payment");
        }
    }
}
