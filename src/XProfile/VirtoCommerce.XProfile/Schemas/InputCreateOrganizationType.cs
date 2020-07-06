using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputCreateOrganizationType : InputObjectGraphType
    {
        public InputCreateOrganizationType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Organization.Name));
            Field<NonNullGraphType<ListGraphType<AddressInputType>>>(nameof(Member.Addresses));
        }
    }
}
