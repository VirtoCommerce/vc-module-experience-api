using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputDeleteContactType : InputObjectGraphType
    {
        public InputDeleteContactType()
        {
            Field<NonNullGraphType<StringGraphType>>("userId");
            Field<NonNullGraphType<StringGraphType>>("contactId");
        }
    }
}
