using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XProfile.Requests;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class CreateUserInvitationsInputType : InputObjectGraphType
    {
        public CreateUserInvitationsInputType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(CreateUserInvitationsCommand.Message));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(CreateUserInvitationsCommand.Roles));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(CreateUserInvitationsCommand.Emails));

            Field<NonNullGraphType<StringGraphType>>(nameof(CreateUserInvitationsCommand.OrganizationId));
            Field<NonNullGraphType<StringGraphType>>(nameof(CreateUserInvitationsCommand.StoreId));
            Field<NonNullGraphType<StringGraphType>>(nameof(CreateUserInvitationsCommand.StoreEmail));
            Field<NonNullGraphType<StringGraphType>>(nameof(CreateUserInvitationsCommand.Language));

        }
    }
}
