using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

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
            ContactAggregate result = null;

            if (!contactId.IsNullOrEmpty())
            {
                var contact = await _memberService.GetByIdAsync(contactId, null, nameof(Contact));
                if (contact != null)
                {
                    result = new ContactAggregate(contact as Contact);
                }
            }

            return result;
        }

        public Task SaveAsync(ContactAggregate contactAggregate)
        {
            return _memberService.SaveChangesAsync(new[] { contactAggregate.Contact });
        }


        public Task DeleteContactAsync(string contactId)
        {
            return _memberService.DeleteAsync(new[] { contactId });
        }
    }
}
