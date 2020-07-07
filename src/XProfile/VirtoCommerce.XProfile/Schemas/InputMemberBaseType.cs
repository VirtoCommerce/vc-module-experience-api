using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public abstract class InputMemberBaseType : InputObjectGraphType
    {
        protected InputMemberBaseType()
        {
            Field<NonNullGraphType<ListGraphType<AddressInputType>>>(nameof(Member.Addresses));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(Member.Phones));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(Member.Emails));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(Member.Groups));
            //TODO
            // Field<NonNullGraphType<ListGraphType<DynamicObjectPropertyInputType>>>(nameof(Member.DynamicProperties));
        }
    }
}
