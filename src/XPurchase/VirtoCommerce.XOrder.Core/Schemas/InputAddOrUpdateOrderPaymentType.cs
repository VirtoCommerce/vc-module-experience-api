using GraphQL.Types;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputAddOrUpdateOrderPaymentType : InputObjectGraphType
    {
        public InputAddOrUpdateOrderPaymentType()
        {
            Field<NonNullGraphType<StringGraphType>>("orderId", "Order ID");
            Field<NonNullGraphType<InputOrderPaymentType>>("payment", "Payment");
        }
    }
}
