using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputSendVerifyEmailType : InputObjectGraphType
    {
        public InputSendVerifyEmailType()
        {
            Field<StringGraphType>("email");
        }
    }
}
