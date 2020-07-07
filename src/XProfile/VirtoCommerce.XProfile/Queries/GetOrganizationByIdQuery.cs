using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetOrganizationByIdQuery : IQuery<OrganizationAggregate>
    {
        public GetOrganizationByIdQuery()
        {

        }

        public GetOrganizationByIdQuery(string organizationId)
        {
            OrganizationId = organizationId;
        }

        public string OrganizationId { get; set; }
    }
}
