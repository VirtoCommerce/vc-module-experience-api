using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateContactAddressType : InputObjectGraphType
    {
        public InputUpdateContactAddressType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.Id));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.Name));
            Field<NonNullGraphType<ListGraphType<AddressInputType>>>(nameof(Member.Addresses));
        }
    }
}
