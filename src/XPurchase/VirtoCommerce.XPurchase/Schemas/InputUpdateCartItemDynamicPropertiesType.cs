using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputUpdateCartItemDynamicPropertiesType : InputCartBaseType
    {
        public InputUpdateCartItemDynamicPropertiesType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId",
                "Line item Id");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties",
                "Dynamic properties");
        }
    }
}
