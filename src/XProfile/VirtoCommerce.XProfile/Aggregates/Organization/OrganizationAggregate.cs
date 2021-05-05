using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class OrganizationAggregate : MemberAggregateRootBase
    {
        public Organization Organization => Member as Organization;
    }
}
