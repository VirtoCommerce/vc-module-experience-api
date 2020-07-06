using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class MemberInputType : InputObjectGraphType<Member>
    {
        public MemberInputType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
            //Field(x => x.MemberType);
            Field(x => x.OuterId, true);
            Field<NonNullGraphType<ListGraphType<AddressInputType>>>(nameof(Member.Addresses));
            Field(x => x.Phones);
            Field(x => x.Emails);
            Field<NonNullGraphType<ListGraphType<NoteInputType>>>(nameof(Member.Notes));
            Field(x => x.Groups);
            // Field<NonNullGraphType<ListGraphType<DynamicObjectPropertyInputType>>>(nameof(Member.DynamicProperties));
        }
    }

}
