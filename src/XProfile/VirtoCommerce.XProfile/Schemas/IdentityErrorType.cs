using GraphQL.Types;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class IdentityErrorType : ObjectGraphType<IdentityError>
    {
        public IdentityErrorType()
        {
            Field(x => x.Code);
            Field(x => x.Description);
        }
    }
}
