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
            Field<StringGraphType>("MemberId", description: "Id of the associated Memeber");
            Field<StringGraphType>("PhoneNumber", description: "User phone number");
            Field<BooleanGraphType>("PhoneNumberConfirmed", description: "Is user phone number confirmed");
            Field<StringGraphType>("PhotoUrl", description: "User photo URL");
            Field<ListGraphType<InputAssignRoleType>>(nameof(ApplicationUser.Roles), description: "List of user roles");
            Field<StringGraphType>("StoreId", description: "Associated store Id");
            Field<BooleanGraphType>("TwoFactorEnabled", description: "Is two factor authentication enabled");
            Field<NonNullGraphType<StringGraphType>>("UserName", description: "User name");
            Field<NonNullGraphType<StringGraphType>>("UserType", description: "User type (Manager, Customer)"); // Manager, Customer
            Field<StringGraphType>("passwordHash", description: "Password hash");
            Field<NonNullGraphType<StringGraphType>>("securityStamp", description: "Security stamp");
        }
    }
}
