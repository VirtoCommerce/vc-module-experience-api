using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputUpdateCartShipmentDynamicPropertiesType : InputCartBaseType
    {
        public InputUpdateCartShipmentDynamicPropertiesType()
        {
            Field<NonNullGraphType<StringGraphType>>("shipmentId",
                "Shipment Id");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties",
                "Dynamic properties");
        }
    }
}
