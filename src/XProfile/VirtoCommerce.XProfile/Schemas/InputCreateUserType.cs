using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputCreateUserType : InputObjectGraphType
    {
        public InputCreateUserType()
        {
            Field<NonNullGraphType<InputCreateApplicationUserType>>("applicationUser", description: "Application user to create");
        }
    }
}
