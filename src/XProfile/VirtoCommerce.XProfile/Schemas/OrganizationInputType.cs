using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class OrganizationInputType : MemberInputType // InputObjectGraphType<Organization>
    {
        public OrganizationInputType()
        {
            Field<StringGraphType>(nameof(Organization.Description));
            Field<StringGraphType>(nameof(Organization.BusinessCategory));
            Field<StringGraphType>(nameof(Organization.OwnerId));
            Field<StringGraphType>(nameof(Organization.ParentId));
        }
    }
}
