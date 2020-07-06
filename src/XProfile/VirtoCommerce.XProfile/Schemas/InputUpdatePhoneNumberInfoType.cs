using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdatePhoneNumberInfoType : InputIdType
    {
        public InputUpdatePhoneNumberInfoType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(ApplicationUser.PhoneNumber));
        }
    }
}
