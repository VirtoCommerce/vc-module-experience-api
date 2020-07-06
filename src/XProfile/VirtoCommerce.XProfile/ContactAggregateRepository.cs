using System;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class ContactAggregateRepository : IContactAggregateRepository
    {
        private readonly IMemberService _memberService;
        private readonly Func<ContactAggregate> _contactAggregateFactory;

        public ContactAggregateRepository(IMemberService memberService, Func<ContactAggregate> contactAggregateFactory)
        {
            _memberService = memberService;
            _contactAggregateFactory = contactAggregateFactory;
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

        public async Task SaveAsync(ContactAggregate contactAggregate)
        {
            await _memberService.SaveChangesAsync(new[] { contactAggregate.Contact });
        }

        protected virtual async Task<ContactAggregate> InnerGetContactByIdAsync(Contact contact)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            var aggregate = _contactAggregateFactory();
            aggregate.SetContact(contact);

            return await Task.FromResult(aggregate);
        }
    }
}
