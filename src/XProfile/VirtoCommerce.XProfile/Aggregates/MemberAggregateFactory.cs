using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class MemberAggregateFactory : IMemberAggregateFactory
    {
        public virtual T Create<T>(Member member) where T : class, IMemberAggregateRoot
        {
            var result = default(T);

            if (member != null)
            {
                result = member.MemberType switch
                {
                    nameof(Organization) => (T)(object)AbstractTypeFactory<OrganizationAggregate>.TryCreateInstance(),
                    _ => (T)(object)AbstractTypeFactory<ContactAggregate>.TryCreateInstance()
                };

                result.Member = member;
            }

            return result;
        }
    }
}
