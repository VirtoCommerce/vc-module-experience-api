using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class MemberAggregateBuilder
    {
        public virtual IMemberAggregateRoot BuildMemberAggregate(Member member)
        {
            IMemberAggregateRoot result = null;

            if (member != null)
            {
                result = member.MemberType switch
                {
                    nameof(Organization) => AbstractTypeFactory<OrganizationAggregate>.TryCreateInstance(),
                    _ => AbstractTypeFactory<ContactAggregate>.TryCreateInstance(),
                };

                result.Member = member;
            }

            return result;
        }
    }
}
