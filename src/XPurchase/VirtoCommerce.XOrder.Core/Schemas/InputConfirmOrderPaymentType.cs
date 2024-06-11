using GraphQL.Types;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputConfirmOrderPaymentType : InputObjectGraphType
    {
        public InputConfirmOrderPaymentType()
        {
            Field<NonNullGraphType<InputOrderPaymentType>>("payment");
        }
    }
}
