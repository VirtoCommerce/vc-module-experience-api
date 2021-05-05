using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public interface IMemberAggregateRootRepository
    {
        Task<IMemberAggregateRoot> GetMemberAggregateRootByIdAsync(string id);
        Task SaveAsync(IMemberAggregateRoot aggregate);
        Task DeleteAsync(string id);
    }
}
