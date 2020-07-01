using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class PhoneNumberUpdateInfoInputType : IdInputType
    {
        public PhoneNumberUpdateInfoInputType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(ApplicationUser.PhoneNumber));
        }
    }
}
