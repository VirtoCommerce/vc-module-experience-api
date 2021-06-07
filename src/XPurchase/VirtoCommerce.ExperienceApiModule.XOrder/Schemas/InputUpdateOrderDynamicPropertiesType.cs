using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputUpdateOrderDynamicPropertiesType : InputObjectGraphType
    {
        public InputUpdateOrderDynamicPropertiesType()
        {
            Field<StringGraphType>("orderId");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties");
        }
    }
}
