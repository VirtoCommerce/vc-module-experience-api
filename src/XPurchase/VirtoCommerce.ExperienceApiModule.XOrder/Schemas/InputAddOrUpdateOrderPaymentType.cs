using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
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
