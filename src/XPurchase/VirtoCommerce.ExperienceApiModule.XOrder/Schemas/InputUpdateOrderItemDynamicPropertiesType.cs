using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputUpdateOrderItemDynamicPropertiesType : InputObjectGraphType
    {
        public InputUpdateOrderItemDynamicPropertiesType()
        {
            Field<StringGraphType>("orderId");
            Field<StringGraphType>("lineItemId");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties");
        }
    }
}
