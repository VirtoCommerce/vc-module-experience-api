using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateContactAddressType : InputObjectGraphType
    {
        public InputUpdateContactAddressType()
        {
            Field<NonNullGraphType<StringGraphType>>("contactId");
            Field<NonNullGraphType<ListGraphType<InputMemberAddressType>>>(nameof(Member.Addresses));
        }
    }
}
