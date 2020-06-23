using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class UserUpdateInfoType : ObjectGraphType<UserUpdateInfo>
    {
        public UserUpdateInfoType()
        {
            Field(x => x.FirstName).Description("First name");
            Field(x => x.LastName).Description("Last name");
            Field(x => x.Email).Description("Email");
            Field(x => x.FullName).Description("FullName");
            Field(x => x.Id).Description("Id");
        }
    }
}
