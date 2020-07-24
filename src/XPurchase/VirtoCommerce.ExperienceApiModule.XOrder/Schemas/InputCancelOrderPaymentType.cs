using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputCancelOrderPaymentType : InputObjectGraphType
    {
        public InputCancelOrderPaymentType()
        {
            Field<NonNullGraphType<InputPaymentInType>>("payment");
        }
    }
}
