using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public abstract class InputMemberBaseType : InputObjectGraphType
    {
        protected InputMemberBaseType()
        {
            Field<StringGraphType>(nameof(Member.Id));
            Field<StringGraphType>(nameof(Member.MemberType));
            Field<NonNullGraphType<ListGraphType<AddressInputType>>>(nameof(Member.Addresses));
            Field<ListGraphType<StringGraphType>>(nameof(Member.Phones));
            Field<ListGraphType<StringGraphType>>(nameof(Member.Emails));
            Field<ListGraphType<StringGraphType>>(nameof(Member.Groups));
            //TODO
            // Field<NonNullGraphType<ListGraphType<DynamicObjectPropertyInputType>>>(nameof(Member.DynamicProperties));
        }
    }
}
