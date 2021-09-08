using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputResetPasswordByTokenType : InputObjectGraphType
    {
        public InputResetPasswordByTokenType()
        {
            Field<NonNullGraphType<StringGraphType>>("token");
            Field<NonNullGraphType<StringGraphType>>("userId");
            Field<NonNullGraphType<StringGraphType>>("newPassword");
        }
    }
}
