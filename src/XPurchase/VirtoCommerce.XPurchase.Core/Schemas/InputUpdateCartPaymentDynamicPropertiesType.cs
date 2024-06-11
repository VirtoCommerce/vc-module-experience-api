using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputUpdateCartPaymentDynamicPropertiesType : InputCartBaseType
    {
        public InputUpdateCartPaymentDynamicPropertiesType()
        {
            Field<NonNullGraphType<StringGraphType>>("paymentId",
                "Payment Id");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>("dynamicProperties",
                "Dynamic properties");
        }
    }
}
