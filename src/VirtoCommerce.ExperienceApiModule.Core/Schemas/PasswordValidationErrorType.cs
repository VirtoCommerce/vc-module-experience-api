using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class PasswordValidationErrorType : ObjectGraphType<IdentityErrorInfo>
    {
        public PasswordValidationErrorType()
        {
            Field(x => x.Code).Description("Error code");
            Field(x => x.ErrorParameter, nullable: true).Description("Error parameter");
            Field(x => x.Description, nullable: true).Description("Error description");
        }
    }
}
