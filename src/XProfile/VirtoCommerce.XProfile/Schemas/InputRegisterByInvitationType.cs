using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputRegisterByInvitationType: InputObjectGraphType
    {
        public InputRegisterByInvitationType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(RegisterByInvitationCommand.UserId));
            Field<NonNullGraphType<StringGraphType>>(nameof(RegisterByInvitationCommand.Token));
            Field<NonNullGraphType<StringGraphType>>(nameof(RegisterByInvitationCommand.FirstName));
            Field<NonNullGraphType<StringGraphType>>(nameof(RegisterByInvitationCommand.LastName));
            Field<NonNullGraphType<StringGraphType>>(nameof(RegisterByInvitationCommand.Username));
            Field<NonNullGraphType<StringGraphType>>(nameof(RegisterByInvitationCommand.Password));
        }
    }
}
