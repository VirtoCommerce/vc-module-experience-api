using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputCreateOrganizationType : InputMemberBaseType
    {
        public InputCreateOrganizationType()
        {
            Field<NonNullGraphType<StringGraphType>>("userId");
        }
    }
}
