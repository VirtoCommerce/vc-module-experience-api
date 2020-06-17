using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.ValidationErrors;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class ValidationErrorType : ObjectGraphType<ValidationError>
    {
        public ValidationErrorType()
        {
            Field(x => x.ErrorCode, nullable: true).Description("Error code");
        }
    }
}
