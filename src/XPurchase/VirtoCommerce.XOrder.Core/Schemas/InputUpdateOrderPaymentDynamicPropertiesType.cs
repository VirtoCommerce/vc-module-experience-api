using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputUpdateOrderPaymentDynamicPropertiesType : InputObjectGraphType
    {
        public InputUpdateOrderPaymentDynamicPropertiesType()
        {
            Field<StringGraphType>("orderId");
            Field<StringGraphType>("paymentId");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties");
        }
    }
}
