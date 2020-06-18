using GraphQL.Types;
using VirtoCommerce.XPurchase.Models.Cart.ValidationErrors;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ValidationErrorType : ObjectGraphType<ValidationError>
    {
        public ValidationErrorType()
        {
            Field(x => x.ErrorCode, nullable: true).Description("Error code");
        }
    }
}
