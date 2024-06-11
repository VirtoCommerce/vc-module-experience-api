using GraphQL.Types;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputInitializePaymentType : InputObjectGraphType
    {
        public InputInitializePaymentType()
        {
            Field<StringGraphType>("orderId", "Order Id");
            Field<NonNullGraphType<StringGraphType>>("paymentId", "Payment Id");
        }
    }
}
