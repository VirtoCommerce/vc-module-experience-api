using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XProfile.Requests;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class GetOrganizationUsersInputType : InputObjectGraphType
    {
        public GetOrganizationUsersInputType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(GetOrganizationUsersCommand.OrganizationId), "Organization ID");
            Field<NonNullGraphType<StringGraphType>>(nameof(GetOrganizationUsersCommand.UserId), "The context user id");
        }
    }
}
