using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateUserType : InputObjectGraphType
    {
        public InputUpdateUserType()
        {
            Field<NonNullGraphType<InputUpdateApplicationUserType>>("applicationUser", description: "Application user to update");
        }
    }
}
