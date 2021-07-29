using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class PasswordValidationResult : ObjectGraphType<PasswordValidationResponse>
    {
        public PasswordValidationResult()
        {
            Field(x => x.Succeeded).Description("Validation result status");
            Field(x => x.ErrorCodes).Description("Error codes");
        }
    }
}
