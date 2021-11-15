using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateMemberAddressType : InputObjectGraphType
    {
        public InputUpdateMemberAddressType()
        {
            Field<NonNullGraphType<StringGraphType>>("memberId",
                "Member Id");
            Field<NonNullGraphType<ListGraphType<InputMemberAddressType>>>(nameof(Member.Addresses),
                "Addresses");
        }
    }
}
