using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateUserType : InputObjectGraphType<ApplicationUser>
    {
        public InputUpdateUserType()
        {
            Field(x => x.Email);
            Field(x => x.Id);
            Field(x => x.IsAdministrator, true);
            Field(x => x.LockoutEnabled, true);
            Field(x => x.LockoutEnd, true);
            Field(x => x.MemberId, true);
            Field(x => x.PhoneNumber, true);
            Field(x => x.PhoneNumberConfirmed, true);
            Field(x => x.PhotoUrl, true);
            Field<ListGraphType<InputAssignRoleType>>(nameof(ApplicationUser.Roles));
            Field(x => x.TwoFactorEnabled, true);
            Field(x => x.UserName);
            Field(x => x.UserType); // Manager, Customer
        }
    }
}
