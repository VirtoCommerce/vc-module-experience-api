using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class OrganizationUpdateInfoInputType : InputObjectGraphType
    {
        public OrganizationUpdateInfoInputType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Organization.Id));
            Field<NonNullGraphType<StringGraphType>>(nameof(Organization.Name));
            Field<NonNullGraphType<ListGraphType<AddressInputType>>>(nameof(Member.Addresses));
            // Field<NonNullGraphType<ListGraphType<DynamicObjectPropertyInputType>>>(nameof(Member.DynamicProperties));
        }
    }

}
