using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class CustomIdentityResultType : ObjectGraphType<IdentityResultResponse>
    {
        public CustomIdentityResultType()
        {
            Field(x => x.Succeeded);
            Field<ListGraphType<IdentityErrorInfoType>>("errors",
                "The errors that occurred during the identity operation.",
                resolve: context => context.Source.Errors);
        }
    }
}
