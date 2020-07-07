using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetContactQueryHandler : IQueryHandler<GetContactByIdQuery, ContactAggregate>
    {
        private readonly IMemberService _memberService;
        public IDictionary<string, Contact> Results { get; set; } = new Dictionary<string, Contact>();

        public GetContactQueryHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task<ContactAggregate> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            return new ContactAggregate(
                await _memberService.GetByIdAsync(request.ContactId, null, nameof(Contact)) as Contact
                );
        }
    }
}
