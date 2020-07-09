using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class OrganizationAggregateRepository : IOrganizationAggregateRepository
    {
        private readonly IMemberService _memberService;

        public OrganizationAggregateRepository(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task<OrganizationAggregate> GetOrganizationByIdAsync(string organizationId)
        {
            var organization = await _memberService.GetByIdAsync(organizationId, null, nameof(Organization));

            if (organization != null)
            {
                return new OrganizationAggregate((Organization)organization);
            }

            return null;
        }

        public async Task<IEnumerable<OrganizationAggregate>> GetOrganizationsByIdsAsync(string[] ids)
        {
            var members = await _memberService.GetByIdsAsync(ids, null, new[] { nameof(Organization) });

            if (members.IsNullOrEmpty())
            {
                return null;
            }
            else
            {
                return members.OfType<Organization>().Select(x=> new OrganizationAggregate(x));
            }
        }

        public async Task SaveAsync(OrganizationAggregate organizationAggregate)
        {
            await _memberService.SaveChangesAsync(new[] { organizationAggregate.Organization });
        }
    
    }
}
