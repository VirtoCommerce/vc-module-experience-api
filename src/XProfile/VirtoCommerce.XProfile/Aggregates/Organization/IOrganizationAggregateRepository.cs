using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public interface IOrganizationAggregateRepository : IMemberAggregateRootRepository
    {
        Task<IEnumerable<OrganizationAggregate>> GetOrganizationsByIdsAsync(string[] ids);
    }
}
