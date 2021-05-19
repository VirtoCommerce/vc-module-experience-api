using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputUpdateCartShipmentDynamicPropertiesType : InputCartBaseType
    {
        public InputUpdateCartShipmentDynamicPropertiesType()
        {
            Field<NonNullGraphType<StringGraphType>>("shipmentId");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties");
        }
    }
}
