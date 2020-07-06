using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public interface IContactAggregateRepository
    {
        Task SaveAsync(ContactAggregate contactAggregate);
        Task<ContactAggregate> GetContactByIdAsync(string contactId);
    }
}
