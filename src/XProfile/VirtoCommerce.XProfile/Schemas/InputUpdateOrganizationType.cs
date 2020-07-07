using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateOrganizationType : InputMemberBaseType
    {
        public InputUpdateOrganizationType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Organization.Id));
            Field<NonNullGraphType<StringGraphType>>(nameof(Organization.Name));
        }
    }

}
