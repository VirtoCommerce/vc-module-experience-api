using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddOrUpdateCartPaymentType : InputCartBaseType
    {
        public InputAddOrUpdateCartPaymentType()
        {
            Field<NonNullGraphType<PaymentType>>("payment");
        }
    }
}
