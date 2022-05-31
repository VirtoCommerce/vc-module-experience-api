using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputAuthorizePaymentType : InputObjectGraphType
    {
        public InputAuthorizePaymentType()
        {
            Field<StringGraphType>("orderId", "Order Id");
            Field<NonNullGraphType<StringGraphType>>("paymentId", "Payment Id");
            //Field<StringGraphType>("paymentMethodCode", "Payment Id");
            Field<ListGraphType<InputKeyValueType>>("parameters", "Input parameters");
        }
    }
}
