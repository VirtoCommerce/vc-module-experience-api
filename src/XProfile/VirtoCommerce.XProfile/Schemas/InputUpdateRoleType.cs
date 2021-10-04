using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateRoleType : InputObjectGraphType
    {
        public InputUpdateRoleType()
        {
            Field<NonNullGraphType<InputUpdateRoleInnerType>>("role", description: "Role to update");
        }
    }
}
