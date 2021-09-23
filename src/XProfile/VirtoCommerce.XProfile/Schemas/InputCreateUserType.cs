using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputCreateUserType : InputObjectGraphType
    {
        public InputCreateUserType()
        {
            Field<StringGraphType>("createdBy");
            Field<DateTimeGraphType>("createdDate");
            Field<NonNullGraphType<StringGraphType>>("email");
            Field<StringGraphType>("id");
            Field<BooleanGraphType>("lockoutEnabled");
            Field<DateTimeGraphType>("LockoutEnd");
            Field<ListGraphType<InputApplicationUserLoginType>>(nameof(ApplicationUser.Logins));
            Field<StringGraphType>("MemberId");
            Field<StringGraphType>("Password"); // nullable, for external logins
            Field<StringGraphType>("PhoneNumber");
            Field<BooleanGraphType>("PhoneNumberConfirmed");
            Field<StringGraphType>("PhotoUrl");
            Field<ListGraphType<InputAssignRoleType>>(nameof(ApplicationUser.Roles));
            Field<StringGraphType>("StoreId");
            Field<BooleanGraphType>("TwoFactorEnabled");
            Field<NonNullGraphType<StringGraphType>>("UserName");
            Field<NonNullGraphType<StringGraphType>>("UserType"); // Manager, Customer
            Field<BooleanGraphType>("PasswordExpired");
        }
    }
}
