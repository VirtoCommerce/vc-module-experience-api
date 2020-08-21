using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateUserType : InputObjectGraphType<ApplicationUser>
    {
        public InputUpdateUserType()
        {
            Field<NonNullGraphType<StringGraphType>>("userId");
            Field(x => x.AccessFailedCount, true);
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
            Field(x => x.StoreId, true);
            Field(x => x.TwoFactorEnabled, true);
            Field(x => x.UserName);
            Field(x => x.UserType); // Manager, Customer
            Field(x => x.PasswordHash, true);
            Field(x => x.SecurityStamp);
        }
    }
}
