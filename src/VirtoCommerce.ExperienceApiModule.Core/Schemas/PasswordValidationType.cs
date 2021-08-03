using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

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


    public class PasswordValidationType : ObjectGraphType<PasswordValidationResponse>
    {


        public PasswordValidationType()
        {
            Field(x => x.Succeeded).Description("Validation result status");
            Field<ListGraphType<PasswordValidationErrorType>>("Errors", description: "Validation errors", resolve: fieldContext => fieldContext.Source.Errors);
        }
    }
}
