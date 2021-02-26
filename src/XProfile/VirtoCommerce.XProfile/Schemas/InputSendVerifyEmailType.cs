using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    class InputSendVerifyEmailType : InputObjectGraphType
    {
        public InputSendVerifyEmailType()
        {
            Field<StringGraphType>("email");
        }
    }
}
