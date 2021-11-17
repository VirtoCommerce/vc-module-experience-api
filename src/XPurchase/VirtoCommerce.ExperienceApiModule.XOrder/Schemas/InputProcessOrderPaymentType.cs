using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputProcessOrderPaymentType : InputObjectGraphType
    {
        public InputProcessOrderPaymentType()
        {
            Field<NonNullGraphType<StringGraphType>>("orderId",
                "Order ID");
            Field<NonNullGraphType<StringGraphType>>("paymentId",
                "Payment ID");
            Field<InputOrderBankCardInfoType>("bankCardInfo",
                "Credit card details");
        }
    }
}
