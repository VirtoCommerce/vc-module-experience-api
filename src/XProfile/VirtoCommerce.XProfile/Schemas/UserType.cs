using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class UserType : ObjectGraphType<ApplicationUser>
    {
        public UserType()
        {
            Field(x => x.AccessFailedCount);
            Field(x => x.CreatedBy);
            Field(x => x.CreatedDate);
            Field(x => x.Email);
            Field(x => x.EmailConfirmed);
            Field(x => x.Id);
            Field(x => x.IsAdministrator);
            Field(x => x.LockoutEnabled);
            Field(x => x.LockoutEnd, true);
            //Field<DateTimeOffsetGraphType>("LockoutEnd", resolve: context => context.Source.LockoutEnd.GetValueOrDefault());
            //Field(x => x.Logins);
            Field("contactId", x => x.MemberId, true);
            Field(x => x.ModifiedBy, true);
            Field(x => x.ModifiedDate, true);
            Field(x => x.NormalizedEmail, true);
            Field(x => x.NormalizedUserName);
            Field(x => x.PasswordExpired);
            Field(x => x.PhoneNumber, true);
            Field(x => x.PhoneNumberConfirmed);
            Field(x => x.PhotoUrl, true);
            Field<ListGraphType<RoleType>>("roles", resolve: x => x.Source.Roles);
            Field(x => x.StoreId, true);
            Field(x => x.TwoFactorEnabled);
            Field(x => x.UserName);
            Field(x => x.UserType);
        }
    }
}
