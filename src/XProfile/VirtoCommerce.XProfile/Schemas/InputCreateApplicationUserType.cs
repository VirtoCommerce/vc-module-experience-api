using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputCreateApplicationUserType : InputObjectGraphType
    {
        public InputCreateApplicationUserType()
        {
            Field<StringGraphType>("createdBy", description: "User name of the creator");
            Field<DateTimeGraphType>("createdDate", description: "Date of user creation");
            Field<NonNullGraphType<StringGraphType>>("email", description: "User Email");
            Field<StringGraphType>("id", description: "User ID");
            Field<BooleanGraphType>("lockoutEnabled", description: "Enables locking out the user");
            Field<DateTimeGraphType>("LockoutEnd", description: "End date of lockout");
            Field<ListGraphType<InputApplicationUserLoginType>>(nameof(ApplicationUser.Logins), description: "External logins");
            Field<StringGraphType>("MemberId", description: "Associated Member ID");
            Field<StringGraphType>("Password", description: "User password (nullable, for external logins)"); // nullable, for external logins
            Field<StringGraphType>("PhoneNumber", description: "User phone number");
            Field<BooleanGraphType>("PhoneNumberConfirmed", description: "Shows whether the user phone number is confirmed");
            Field<StringGraphType>("PhotoUrl", description: "User pic URL");
            Field<ListGraphType<InputAssignRoleType>>(nameof(ApplicationUser.Roles), description: "List of user roles");
            Field<StringGraphType>("StoreId", description: "Associated store ID");
            Field<BooleanGraphType>("TwoFactorEnabled", description: "Enables two factor authentication");
            Field<NonNullGraphType<StringGraphType>>("UserName", description: "User name");
            Field<NonNullGraphType<StringGraphType>>("UserType", description: "User type (Manager, Customer)"); // Manager, Customer
            Field<BooleanGraphType>("PasswordExpired", description: "Password expiration date");
        }
    }
}
