using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class PasswordValidationType : ObjectGraphType<PasswordValidationResponse>
    {


        public PasswordValidationType()
        {
            Field(x => x.Succeeded).Description("Validation result status");
            Field<ListGraphType<PasswordValidationErrorType>>("Errors", description: "Validation errors", resolve: fieldContext => fieldContext.Source.Errors);
        }
    }
}
