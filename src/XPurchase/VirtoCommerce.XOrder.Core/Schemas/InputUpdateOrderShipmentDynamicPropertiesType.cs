using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputUpdateOrderShipmentDynamicPropertiesType : InputObjectGraphType
    {
        public InputUpdateOrderShipmentDynamicPropertiesType()
        {
            Field<StringGraphType>("orderId");
            Field<StringGraphType>("shipmentId");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties");
        }
    }
}
