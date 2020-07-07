using System;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class ContactAggregateRepository : IContactAggregateRepository
    {
        private readonly IMemberService _memberService;

        public ContactAggregateRepository(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task<ContactAggregate> GetContactByIdAsync(string contactId)
        {
            var contact = await _memberService.GetByIdAsync(contactId, null, nameof(Contact));

            if (contact != null)
            {
                return await InnerGetContactByIdAsync((Contact)contact);
            }

            return null;
        }

        public Task SaveAsync(ContactAggregate contactAggregate)
        {
            return _memberService.SaveChangesAsync(new[] { contactAggregate.Contact });
        }

        protected virtual async Task<ContactAggregate> InnerGetContactByIdAsync(Contact contact)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            var aggregate = new ContactAggregate(contact);

            return await Task.FromResult(aggregate);
        }

        public Task DeleteContactAsync(string contactId)
        {
            return _memberService.DeleteAsync(new[] { contactId });
        }
    }
}
