using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
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
