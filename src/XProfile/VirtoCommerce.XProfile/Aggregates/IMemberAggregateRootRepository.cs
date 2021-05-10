using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public interface IMemberAggregateRootRepository
    {
        Task<T> GetMemberAggregateRootByIdAsync<T>(string id) where T : class, IMemberAggregateRoot;
        Task SaveAsync(IMemberAggregateRoot aggregate);
        Task DeleteAsync(string id);
    }
}
