using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateRoleType : InputObjectGraphType
    {
        public InputUpdateRoleType()
        {
            Field<NonNullGraphType<InputRoleToUpdate>>("role", description: "Role to update");
        }
    }
}
