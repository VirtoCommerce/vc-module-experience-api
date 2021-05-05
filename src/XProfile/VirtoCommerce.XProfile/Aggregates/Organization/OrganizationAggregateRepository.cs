using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class OrganizationAggregateRepository : MemberAggregateRootRepository, IOrganizationAggregateRepository
    {
        public OrganizationAggregateRepository(IMemberService memberService, MemberAggregateBuilder builder)
            : base(memberService, builder)
        {
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
                return members.OfType<Organization>().Select(x => (OrganizationAggregate)_builder.BuildMemberAggregate(x));
            }
        }
    }
}
