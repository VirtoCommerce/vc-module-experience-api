using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputSearchOrganizationMembersType : InputObjectGraphType
    {
        public InputSearchOrganizationMembersType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(SearchOrganizationMembersQuery.OrganizationId), "Organization ID");
            Field<StringGraphType>(nameof(SearchOrganizationMembersQuery.SearchPhrase));
        }
    }
}
