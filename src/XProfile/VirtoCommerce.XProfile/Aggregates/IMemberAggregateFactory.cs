using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public interface IMemberAggregateFactory
    {
        T Create<T>(Member member) where T : class, IMemberAggregateRoot;
    }
}
