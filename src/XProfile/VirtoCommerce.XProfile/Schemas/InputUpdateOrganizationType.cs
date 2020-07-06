using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateOrganizationType : InputObjectGraphType
    {
        public InputUpdateOrganizationType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Organization.Id));
            Field<NonNullGraphType<StringGraphType>>(nameof(Organization.Name));
            Field<NonNullGraphType<ListGraphType<InputAddressType>>>(nameof(Member.Addresses));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(Member.Phones));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(Member.Emails));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(Member.Groups));
            //TODO
            // Field<NonNullGraphType<ListGraphType<DynamicObjectPropertyInputType>>>(nameof(Member.DynamicProperties));
        }
    }

}
