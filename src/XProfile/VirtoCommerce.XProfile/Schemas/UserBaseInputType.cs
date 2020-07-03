using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class UserBaseInputType : InputObjectGraphType
    {
        public UserBaseInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("userId");
        }
    }

}
