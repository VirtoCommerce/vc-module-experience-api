using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetOrganizationByIdQuery : IQuery<OrganizationAggregate>
    {
        public GetOrganizationByIdQuery(string organizationId, string userId)
        {
            OrganizationId = organizationId;
            UserId = userId;
        }

        public string OrganizationId { get; set; }
        public string UserId { get; set; }
    }
}
