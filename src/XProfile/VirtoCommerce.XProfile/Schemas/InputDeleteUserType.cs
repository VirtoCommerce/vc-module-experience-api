using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputDeleteUserType : InputObjectGraphType
    {
        public InputDeleteUserType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("userNames",
                "User names");
        }
    }
}
