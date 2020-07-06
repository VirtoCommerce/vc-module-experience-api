using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputIdType : InputObjectGraphType
    {
        public InputIdType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(ApplicationUser.Id));
        }
    }
}
