using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateMemberDynamicPropertiesType : InputObjectGraphType
    {
        public InputUpdateMemberDynamicPropertiesType()
        {
            Field<NonNullGraphType<StringGraphType>>("memberId");
            Field<NonNullGraphType<ListGraphType<InputDynamicPropertyValueType>>>(nameof(Member.DynamicProperties));
        }
    }
}
