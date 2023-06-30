using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputConfirmOrderPaymentType : InputObjectGraphType
    {
        public InputConfirmOrderPaymentType()
        {
            Field<NonNullGraphType<InputOrderPaymentType>>("payment");
        }
    }
}
