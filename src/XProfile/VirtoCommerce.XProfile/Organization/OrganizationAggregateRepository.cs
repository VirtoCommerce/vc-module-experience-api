using System;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;

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
                return await InnerGetOrganizationByIdAsync((Organization)organization);
            }

            return null;
        }

        public async Task SaveAsync(OrganizationAggregate organizationAggregate)
        {
            await _memberService.SaveChangesAsync(new[] { organizationAggregate.Organization });
        }

        protected virtual async Task<OrganizationAggregate> InnerGetOrganizationByIdAsync(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            var aggregate = new OrganizationAggregate(organization);
            
            return await Task.FromResult(aggregate);
        }
    }
}
