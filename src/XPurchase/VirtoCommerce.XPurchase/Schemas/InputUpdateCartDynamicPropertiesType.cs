using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputUpdateCartDynamicPropertiesType : InputCartBaseType
    {
        public InputUpdateCartDynamicPropertiesType()
        {
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties");
        }
    }
}
