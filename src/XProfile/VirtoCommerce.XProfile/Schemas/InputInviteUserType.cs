using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputInviteUserType : InputObjectGraphType
    {
        public InputInviteUserType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(InviteUserCommand.StoreId));
            Field<NonNullGraphType<StringGraphType>>(nameof(InviteUserCommand.OrganizationId));
            Field<NonNullGraphType<StringGraphType>>(nameof(InviteUserCommand.UrlSuffix));
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>(nameof(InviteUserCommand.Emails));
            Field<StringGraphType>(nameof(InviteUserCommand.Message));
        }
    }
}
