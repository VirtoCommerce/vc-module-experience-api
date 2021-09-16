using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class IdentityErrorInfoType : ObjectGraphType<IdentityErrorInfo>
    {
        public IdentityErrorInfoType()
        {
            Field(x => x.Code).Description("Error code");
            Field(x => x.ErrorParameter, nullable: true).Description("Error parameter");
            Field(x => x.Description, nullable: true).Description("Error description");
        }
    }
}
