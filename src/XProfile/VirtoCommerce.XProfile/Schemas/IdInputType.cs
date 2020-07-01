using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class IdInputType : InputObjectGraphType
    {
        public IdInputType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(ApplicationUser.Id));
        }
    }
}
