using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class IdentityResultType : ObjectGraphType<IdentityResultResponse>
    {


        public IdentityResultType()
        {
            Field(x => x.Succeeded).Description("Validation result status");
            Field<ListGraphType<IdentityErrorInfoType>>("Errors", description: "Validation errors", resolve: fieldContext => fieldContext.Source.Errors);
        }
    }
}
