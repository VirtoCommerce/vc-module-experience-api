using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    //TODO: We mustn't use such general  commands that update entire contact on the xApi level. Need to use commands more close to real business scenarios instead.
    //remove in the future
    public class InputUpdateUserType : InputObjectGraphType
    {
        public InputUpdateUserType()
        {
            Field<IntGraphType>("accessFailedCount");
            Field<NonNullGraphType<StringGraphType>>("email");
            Field<NonNullGraphType<StringGraphType>>("id");
            Field<BooleanGraphType>("lockoutEnabled");
            Field<DateTimeGraphType>("LockoutEnd");
            Field<StringGraphType>("MemberId");
            Field<StringGraphType>("PhoneNumber");
            Field<BooleanGraphType>("PhoneNumberConfirmed");
            Field<StringGraphType>("PhotoUrl");
            Field<ListGraphType<InputAssignRoleType>>(nameof(ApplicationUser.Roles));
            Field<StringGraphType>("StoreId");
            Field<BooleanGraphType>("TwoFactorEnabled");
            Field<NonNullGraphType<StringGraphType>>("UserName");
            Field<NonNullGraphType<StringGraphType>>("UserType"); // Manager, Customer
            Field<StringGraphType>("passwordHash");
            Field<NonNullGraphType<StringGraphType>>("securityStamp");
        }
    }
}
