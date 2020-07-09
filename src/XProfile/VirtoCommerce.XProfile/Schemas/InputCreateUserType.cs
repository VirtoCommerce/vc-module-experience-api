using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputCreateUserType : InputObjectGraphType<ApplicationUser>
    {
        public InputCreateUserType()
        {
            Field(x => x.CreatedBy, true);
            Field(x => x.CreatedDate, true);
            Field(x => x.Email);
            Field(x => x.Id, true);
            Field(x => x.IsAdministrator, true);
            Field(x => x.LockoutEnabled, true);
            Field(x => x.LockoutEnd, true);
            Field(x => x.MemberId, true);
            Field(x => x.Password);
            Field(x => x.PhoneNumber, true);
            Field(x => x.PhoneNumberConfirmed, true);
            Field(x => x.PhotoUrl, true);
            Field<ListGraphType<InputAssignRoleType>>(nameof(ApplicationUser.Roles));
            Field(x => x.StoreId, true);
            Field(x => x.TwoFactorEnabled, true);
            Field(x => x.UserName);
            Field(x => x.UserType); // Manager, Customer
        }
    }
}
