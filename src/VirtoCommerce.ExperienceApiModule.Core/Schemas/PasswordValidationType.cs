using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class PasswordValidationType : ObjectGraphType<PasswordValidationResponse>
    {
        public PasswordValidationType()
        {
            Field(x => x.Succeeded).Description("Validation result status");
            Field(x => x.ErrorCodes).Description("Error codes");
        }
    }
}
