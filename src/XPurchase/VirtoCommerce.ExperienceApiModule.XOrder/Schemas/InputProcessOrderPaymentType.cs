using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputProcessOrderPaymentType : InputObjectGraphType
    {
        public InputProcessOrderPaymentType()
        {
            Field<NonNullGraphType<StringGraphType>>("orderId");
            Field<NonNullGraphType<StringGraphType>>("paymentId");
            Field<InputOrderBankCardInfoType>("bankCardInfo");
        }
    }
}
