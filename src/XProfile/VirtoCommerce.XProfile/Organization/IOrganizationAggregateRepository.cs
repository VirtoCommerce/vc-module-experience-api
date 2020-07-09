using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public interface IOrganizationAggregateRepository
    {
        Task SaveAsync(OrganizationAggregate organizationAggregate);
        Task<OrganizationAggregate> GetOrganizationByIdAsync(string organizationId);
        Task<IEnumerable<OrganizationAggregate>> GetOrganizationsByIdsAsync(string[] ids);
    }
}
