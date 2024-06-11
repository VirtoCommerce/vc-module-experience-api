using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputUpdateCartDynamicPropertiesType : InputCartBaseType
    {
        public InputUpdateCartDynamicPropertiesType()
        {
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties",
                "Dynamic properties");
        }
    }
}
