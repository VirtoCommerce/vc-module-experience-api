using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateApplicationUserType : InputObjectGraphType
    {
        public InputUpdateApplicationUserType()
        {
            Field<IntGraphType>("accessFailedCount", description: "Failed login attempts for the current user");
            Field<NonNullGraphType<StringGraphType>>("email", description: "User Email");
            Field<NonNullGraphType<StringGraphType>>("id", description: "User ID");
            Field<BooleanGraphType>("lockoutEnabled", description: "Can user be locked out");
            Field<DateTimeGraphType>("LockoutEnd", description: "End date of lockout");
            Field<StringGraphType>("MemberId", description: "Associated Member ID");
            Field<StringGraphType>("PhoneNumber", description: "User phone number");
            Field<BooleanGraphType>("PhoneNumberConfirmed", description: "Displays whether the user phone number is confirmed");
            Field<StringGraphType>("PhotoUrl", description: "User pic URL");
            Field<ListGraphType<InputAssignRoleType>>(nameof(ApplicationUser.Roles), description: "List of user roles");
            Field<StringGraphType>("StoreId", description: "Associated store ID");
            Field<BooleanGraphType>("TwoFactorEnabled", description: "Enables two factor authentication");
            Field<NonNullGraphType<StringGraphType>>("UserName", description: "User name");
            Field<NonNullGraphType<StringGraphType>>("UserType", description: "User type (Manager, Customer)"); // Manager, Customer
            Field<StringGraphType>("passwordHash", description: "Password hash");
            Field<NonNullGraphType<StringGraphType>>("securityStamp", description: "Security stamp");
        }
    }
}
