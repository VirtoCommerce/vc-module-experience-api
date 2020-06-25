using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class UserUpdateInfoInputType : InputObjectGraphType
    {
        public UserUpdateInfoInputType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(ApplicationUser.Id));
            Field<NonNullGraphType<StringGraphType>>(nameof(ApplicationUser.Email));
            Field<ListGraphType<StringGraphType>>(nameof(ApplicationUser.Roles));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FirstName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.LastName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FullName));
        }
    }
}
