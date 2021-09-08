using GraphQL.Types;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class IdentityResultType : ObjectGraphType<IdentityResult>
    {
        public IdentityResultType()
        {
            Field(x => x.Succeeded);
            Field<ListGraphType<IdentityErrorType>>("errors", "The errors that occurred during the identity operation.", resolve: context => context.Source.Errors);
        }
    }
}
