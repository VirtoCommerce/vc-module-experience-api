using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputCreateApplicationUserType : InputObjectGraphType
    {
        public InputCreateApplicationUserType()
        {
            Field<StringGraphType>("createdBy", description: "Username of the creator");
            Field<DateTimeGraphType>("createdDate", description: "Date of user creation");
            Field<NonNullGraphType<StringGraphType>>("email", description: "User Email");
            Field<StringGraphType>("id", description: "User ID");
            Field<BooleanGraphType>("lockoutEnabled", description: "Can user be locked out");
            Field<DateTimeGraphType>("LockoutEnd", description: "End date of lockout");
            Field<ListGraphType<InputApplicationUserLoginType>>(nameof(ApplicationUser.Logins), description: "External logins");
            Field<StringGraphType>("MemberId", description: "Id of the associated Member");
            Field<StringGraphType>("Password", description: "User password (nullable, for external logins)"); // nullable, for external logins
            Field<StringGraphType>("PhoneNumber", description: "User phone number");
            Field<BooleanGraphType>("PhoneNumberConfirmed", description: "Is user phone number confirmed");
            Field<StringGraphType>("PhotoUrl", description: "User photo URL");
            Field<ListGraphType<InputAssignRoleType>>(nameof(ApplicationUser.Roles), description: "List of user roles");
            Field<StringGraphType>("StoreId", description: "Associated store Id");
            Field<BooleanGraphType>("TwoFactorEnabled", description: "Is two factor authentication enabled");
            Field<NonNullGraphType<StringGraphType>>("UserName", description: "User name");
            Field<NonNullGraphType<StringGraphType>>("UserType", description: "User type (Manager, Customer)"); // Manager, Customer
            Field<BooleanGraphType>("PasswordExpired", description: "Password expiration date");
        }
    }
}
