using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputProcessOrderPaymentType : InputObjectGraphType
    {
        public InputProcessOrderPaymentType()
        {
            Field<NonNullGraphType<StringGraphType>>("orderId",
                "Order Id");
            Field<NonNullGraphType<StringGraphType>>("paymentId",
                "Payment Id");
            Field<InputOrderBankCardInfoType>("bankCardInfo",
                "Bank card info");
        }
    }
}
