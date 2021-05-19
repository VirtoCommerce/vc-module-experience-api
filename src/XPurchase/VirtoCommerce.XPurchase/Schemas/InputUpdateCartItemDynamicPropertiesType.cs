using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputUpdateCartItemDynamicPropertiesType : InputCartBaseType
    {
        public InputUpdateCartItemDynamicPropertiesType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties");
        }
    }
}
